using System;
using System.IO;

namespace ConfigBuilder
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Outputting Entity Configuration");

            FileStream fileStream = File.OpenRead(args[0]);
            StreamReader reader = new StreamReader(fileStream);
            int lineCount = 1;

            while (!reader.EndOfStream)
            {
                // Skip Header Row
                if (lineCount == 1)
                {
                    lineCount++;
                    continue;
                }

                var line = reader.ReadLine();
                var values = line.Split(',');

                Console.WriteLine($"// SQL Data Type: {values[5]}");
                Console.WriteLine($@"builder.Property(prop => prop.{PropertyName(values[3])})
                    .HasColumnName(""{values[3]}"")
                    {ColumnLength(values)}
                    .{IsRequired(values[10])};");

                Console.WriteLine("");
                Console.WriteLine("");
            }
        }

        static string PropertyName(string columnName)
        {
            string[] parts = columnName.Split('_');
            int i = 0;

            foreach (var item in parts)
            {
                parts[i] = char.ToUpper(item[0]) + item.Substring(1);
                i++;
            }

            return string.Join("", parts);
        }

        static string ColumnLength(string[] line)
        {
            string value;

            if (line[5] == "decimal")
            {
                value = $@".HasColumnType(""decimal({line[6]},{line[8]})"")";
            } else if (line[5] == "varchar" || line[5] == "char")
            {
                value = $@".HasMaxLength({line[7]})";
            }
            else
            {
                value = null;
            }

            return value;
        }

        static string IsRequired(string bit)
        {
            return bit == "0" ? "IsRequired()" : "IsRequired(false)";
        }
    }
}
