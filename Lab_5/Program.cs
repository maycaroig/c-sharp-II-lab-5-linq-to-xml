using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;
using System.Reflection;
using System.IO;

namespace Lab4
{
	class Program
	{
		private const string FILENAME = "student.xml";
		private const string SEP = "====================";
		private const long RECORDS = 1500;

		static void Main(string[] args)
		{
			// Setup Console
			Console.WindowWidth = 100;
			Console.WindowHeight = 50;
			Console.BufferHeight = 9999;
			
			// Generate students
			GenerateStudents();

			// Open student file
			XDocument doc = OpenStudentFile();

			// Peform queries
			FindAllGradeSchoolStudents(doc);

			FindValedictorianAndSalutatorian(doc);

			GpaStatisticsPerGradeLevel(doc);

			// Wait for completion
			Console.ReadLine();
		}

		private static void FindAllGradeSchoolStudents(XDocument doc)
		{
            var query = from student in doc.Descendants("student")
                        let grade = (Student.GradeEnum)Enum.Parse(typeof(Student.GradeEnum),
                            student.Element("grade").Value)
                        where grade > Student.GradeEnum.Kindergarten && grade <= Student.GradeEnum.Sixth 
                        orderby grade, student.Element("lastName").Value, student.Element("firstName").Value 
                        select Student.FromXElement(student);

            PrintHeader("FindAllGradeSchoolStudents");

            foreach (Student s in query) 
            { 
                Console.WriteLine(s);
            }
		}

		private static void FindValedictorianAndSalutatorian(XDocument doc)
		{
            var query = (from student in doc.Descendants("student")
                         let grade = (Student.GradeEnum)Enum.Parse(typeof(Student.GradeEnum), 
                            student.Element("grade").Value)
                         let gpa = float.Parse(student.Element("gpa").Value) 
                         where grade == Student.GradeEnum.Senior
                         orderby gpa descending 
                         select Student.FromXElement(student)).Take(2);

            PrintHeader("FindValedictorianAndSalutatorian");

            if (query.Count() == 2) 
            {
                Console.WriteLine("Valedictorian");
                Console.WriteLine(query.ElementAt(0)); 
                Console.WriteLine("Salutatorian");
                Console.WriteLine(query.ElementAt(1));
            }
		}

		private static void GpaStatisticsPerGradeLevel(XDocument doc)
		{
            var query = from student in doc.Descendants("student") 
                        let grade = (Student.GradeEnum)Enum.Parse(typeof(Student.GradeEnum), 
                            student.Element("grade").Value) 
                        orderby float.Parse(student.Element("gpa").Value)
                        group student by grade into g 
                        orderby g.Key 
                        select g;
            
            PrintHeader("GpaStatisticsPerGradeLevel", false);

            foreach (var group in query)
            { 
                Console.WriteLine(group.Key);
                Console.WriteLine("------------");

                float min = group.Min(student => float.Parse(student.Element("gpa").Value)); 
                float max = group.Max(student => float.Parse(student.Element("gpa").Value)); 
                float avg = group.Average(student => float.Parse(student.Element("gpa").Value));
                Console.WriteLine(" Students: {0,4} GPA (min/max/avg): {1:0.000} / {2:0.000} / {3:0.000}", 
                    group.Count(), min, max, avg); 
                Console.WriteLine(); 
            }
		}

		/// <summary>
		/// Prints the header for the given method
		/// </summary>
		/// <param name="title">Title for the header</param>
		private static void PrintHeader(string title, bool showStudentHeader = true)
		{
			Console.WriteLine();
			Console.WriteLine();
			Console.WriteLine("--- " + title + " ---");
			Console.WriteLine();
			if (showStudentHeader)
			{
				Console.WriteLine(Student.HeaderString);
			}
		}

		#region IO Methods
		/// <summary>
		/// Opens the student file
		/// </summary>
		/// <returns></returns>
		private static XDocument OpenStudentFile()
		{
			return XDocument.Load(FILENAME);
		}

		/// <summary>
		/// Generates a new student file
		/// </summary>
		private static void GenerateStudents()
		{
			Random rnd = new Random();
			List<Student> students = new List<Student>();
			List<string> firstNames = new List<string>();
			List<string> lastNames = new List<string>();

			Assembly asm = Assembly.GetExecutingAssembly();

			// First Names
			using (StreamReader sr = new StreamReader(asm.GetManifestResourceStream("Lab5.FirstNames.txt")))
			{
				while (!sr.EndOfStream)
				{
					string name = sr.ReadLine().Trim().ToLower();
					if (!string.IsNullOrEmpty(name))
					{
						firstNames.Add(string.Format("{0}{1}", char.ToUpper(name[0]), name.Substring(1)));
					}
				}
			}

			// Last Names
			using (StreamReader sr = new StreamReader(asm.GetManifestResourceStream("Lab5.LastNames.txt")))
			{
				while (!sr.EndOfStream)
				{
					string name = sr.ReadLine().Trim().ToLower();
					if (!string.IsNullOrEmpty(name))
					{
						lastNames.Add(string.Format("{0}{1}", char.ToUpper(name[0]), name.Substring(1)));
					}
				}
			}

			// Create list of random people
			const long INCREMENT = 999999999L / RECORDS - 100L;
			long id = INCREMENT;
			for (long i = 0; i < RECORDS; i++)
			{
				id += INCREMENT + rnd.Next(1, 100);  // Add some randomness to IDs
				string lastName = lastNames[rnd.Next(0, lastNames.Count)];
				string firstName = firstNames[rnd.Next(0, firstNames.Count)];
				DateTime oldest = DateTime.Now.Subtract(TimeSpan.FromDays(365 * 22));
				DateTime youngest = DateTime.Now.Subtract(TimeSpan.FromDays(365 * 2));
				double oaDate = ((double)rnd.Next((int)oldest.ToOADate(), (int)youngest.ToOADate())) + rnd.NextDouble();
				DateTime dob = DateTime.FromOADate(oaDate);
				float gpa = (float)Math.Round(rnd.NextDouble() * 3.0 + 1.0, 3);
				Student student = new Student(id, Student.GetGrade(dob), lastName, firstName, dob, gpa);
				students.Add(student);
			}

			// Save the student list to a file
			XDocument doc = CreateXDocument(students);
			doc.Save(FILENAME, SaveOptions.None);
		}

		private static XDocument CreateXDocument(List<Student> students)
		{
            XDocument doc = new XDocument(
                new XElement("students", 
                    students.Select((item) => new XElement("student",
                        new XAttribute("id", item.ID),
                        new XElement("lastName", item.LastName),
                        new XElement("firstName", item.FirstName),
                        new XElement("dob", item.DOB),
                        new XElement("gpa", item.GPA),
                        new XElement("grade", item.Grade))
                    )
                )
             ); 
            return doc;
		}
		#endregion IO Methods
	}
}
