using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Linq;
using System.Text;

namespace Lab4
{
	[Serializable]
	public class Student
	{
		public enum GradeEnum
		{
			PreSchool = -1,
            Kindergarten = 0,
            First = 1,
            Second,
            Third,
            Fourth,
            Fifth,
            Sixth,
            Seventh,
            Eighth,
            Freshmen = 9,
            Sophomore = 10,
            Junior = 11,
            Senior = 12,
            College = 13
		 }

		#region Fields

        private long m_ID = 0;
        private string m_LastName = string.Empty;
        private string m_FirstName = string.Empty;
        private DateTime m_DOB = DateTime.MinValue;
        private float m_GPA = 0f;
        private GradeEnum m_Grade = GradeEnum.PreSchool;
     
        #endregion Fields

		#region Properties
        
        public long ID
        {
            get { return m_ID; }
            set 
            {
                if(value >=0)
                m_ID = value;
            }
        }

        public string LastName
        {
            get { return m_LastName; }
            set { m_LastName = value; }
        }

        public string FirstName
        {
            get { return m_FirstName; }
            set { m_FirstName = value; }
        }

        public DateTime DOB
        {
            get { return m_DOB; }
            set { m_DOB = value; }
        }

        public float GPA
        {
            get { return m_GPA; }
            set { m_GPA = value; }
        }

        public GradeEnum Grade
        {
            get { return m_Grade; }
            set { m_Grade = value; }
        }

        public int Age
        {
            get { return GetAge(DOB); }
        }

		#endregion Properties

		#region Constructors

		public Student()
		{
			// No operation
		}

		public Student(long id, GradeEnum grade, string lastName, string firstName, DateTime dob, float gpa)
		{
            ID = id;
            Grade = grade;
            LastName = lastName;
            FirstName = firstName;
            DOB = dob;
            GPA = gpa;
		}

		#endregion Constructors

		#region Methods
		/// <summary>
		/// Gets the age of a person with the given birthdate
		/// </summary>
		/// <param name="dob">Date of birth</param>
		/// <returns>Current age of person</returns>
		public static int GetAge(DateTime dob)
		{
            DateTime now = DateTime.Now;
            int years = now.Year - dob.Year;
            if (now.Month < dob.Month || ((now.Month == dob.Month) && (now.Day < dob.Day)))
                years --;
            return years;
		}

		/// <summary>
		/// Gets the estimated grade level of a person with the given birthdate
		/// </summary>
		/// <param name="dob">Date of birth</param>
		/// <returns>Estimated grade level</returns>
		public static GradeEnum GetGrade(DateTime dob)
		{
            GradeEnum grade = new GradeEnum();

            int age = GetAge(dob);
            if (age < 5)
                grade = GradeEnum.PreSchool;
            else if (age == 5)
                grade = GradeEnum.Kindergarten;
            else if (age >= 18)
                grade = GradeEnum.College;
            else
            {
                if (age == 6)
                    grade = GradeEnum.First;
                if (age == 7)
                    grade = GradeEnum.Second;
                if (age == 8)
                    grade = GradeEnum.Third;
                if(age == 9)
                    grade = GradeEnum.Fourth;
                if (age == 10)
                    grade = GradeEnum.Fifth;
                if (age == 11)
                    grade = GradeEnum.Sixth;
                if (age == 12)
                    grade = GradeEnum.Seventh;
                if (age == 13)
                    grade = GradeEnum.Eighth;
                if (age == 14)
                    grade = GradeEnum.Freshmen;
                if (age == 15)
                    grade = GradeEnum.Sophomore;
                if (age == 16)
                    grade = GradeEnum.Junior;
                if (age == 17)
                    grade = GradeEnum.Senior;
                        
            }

            return grade;
		}

		/// <summary>
		/// Parses a Student object from the given XElement
		/// </summary>
		/// <param name="element">element to parse</param>
		/// <returns>Parsed student</returns>
		public static Student FromXElement(XElement element)
		{
            long id = long.Parse(element.Attribute("id").Value);
            string lastName = element.Elements("lastName").FirstOrDefault().Value;
            string firstName = element.Elements("firstName").FirstOrDefault().Value;
            DateTime dob = DateTime.Parse(element.Elements("dob").FirstOrDefault().Value);
            float gpa = float.Parse(element.Elements("gpa").FirstOrDefault().Value);
            GradeEnum grade = (GradeEnum)Enum.Parse(typeof(GradeEnum), element.Elements("grade").FirstOrDefault().Value);

            return new Student(id, grade, lastName, firstName, dob, gpa);

		}

		/// <summary>
		/// Student header
		/// </summary>
		public static string HeaderString
		{
			get
			{
				// GRADExxxxxxx 000-00-0000 LASTNAMExxxxxxxxxxxxxxxxx FIRSTNAMExxxxxxxxxxxxxxxx 00/00/0000 0.000
				// Grade        ID          Last Name                 First Name                Birth Date GPA
				// =============================================================================================
				return "Grade        ID          Last Name                 First Name                Birth Date GPA" + Environment.NewLine + new string('=', 93);
			}
		}

		public override string ToString()
		{
            return string.Format("{0,-12} {1:000-00-0000} {2,-25} {3,-25} {4:MM/dd/yyyy} {5:0.000}",
                Grade, ID, LastName, FirstName, DOB, GPA);
		}

		#endregion Methods
	}
}
