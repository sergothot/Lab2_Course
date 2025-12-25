namespace Lab2_Course;

public class Teacher
{
	public Guid Id { get; } = Guid.NewGuid();
	public string Name { get; }

	public Teacher(string name)
	{
		Name = name;
	}
}

public class Student
{
	public Guid Id { get; } = Guid.NewGuid();
	public string Name { get; }

	public Student(string name)
	{
		Name = name;
	}
}

public abstract class Course
{
	public Guid Id { get; } = Guid.NewGuid();
	public string Title { get; }
	public Teacher? Teacher { get; private set; }
	public List<Student> Students { get; } = new();

	protected Course(string title)
	{
		Title = title;
	}

	public void AssignTeacher(Teacher teacher)
	{
		Teacher = teacher;
	}

	public bool AddStudent(Student student)
	{
		if (Students.Exists(s => s.Id == student.Id))
		{
			return false;
		}

		Students.Add(student);
		return true;
	}

	public bool RemoveStudent(Guid studentId)
	{
		var removed = Students.RemoveAll(s => s.Id == studentId);
		return removed > 0;
	}

	public abstract string Describe();
}

public class OnlineCourse : Course
{
	public string Platform { get; }

	public OnlineCourse(string title, string platform) : base(title)
	{
		Platform = platform;
	}

	public override string Describe()
	{
		var teacherPart = Teacher != null ? $"Преподаватель: {Teacher.Name}" : "Преподаватель: TBD";
		return $"Онлайн: {Title} ({Platform}), {teacherPart}, Студенты: {Students.Count}";
	}
}

public class OfflineCourse : Course
{
	public string Classroom { get; }

	public OfflineCourse(string title, string classroom) : base(title)
	{
		Classroom = classroom;
	}

	public override string Describe()
	{
		var teacherPart = Teacher != null ? $"Преподаватель: {Teacher.Name}" : "Преподаватель: TBD";
		return $"Офлайн: {Title} (Аудитория {Classroom}), {teacherPart}, Студенты: {Students.Count}";
	}
}

public class CourseManager
{
	private readonly List<Course> _courses = new();

	public void AddCourse(Course course)
	{
		_courses.Add(course);
	}

	public bool RemoveCourse(Guid courseId)
	{
		var removed = _courses.RemoveAll(c => c.Id == courseId);
		return removed > 0;
	}

	public bool AssignTeacher(Guid courseId, Teacher teacher)
	{
		var course = _courses.Find(c => c.Id == courseId);
		if (course is null)
		{
			return false;
		}

		course.AssignTeacher(teacher);
		return true;
	}

	public bool EnrollStudent(Guid courseId, Student student)
	{
		var course = _courses.Find(c => c.Id == courseId);
		if (course is null)
		{
			return false;
		}

		return course.AddStudent(student);
	}

	public bool UnenrollStudent(Guid courseId, Guid studentId)
	{
		var course = _courses.Find(c => c.Id == courseId);
		if (course is null)
		{
			return false;
		}

		return course.RemoveStudent(studentId);
	}

	public IReadOnlyList<Course> GetCoursesByTeacher(Guid teacherId)
	{
		return _courses.Where(c => c.Teacher?.Id == teacherId).ToList();
	}

	public IReadOnlyList<Course> ListAll()
	{
		return _courses.AsReadOnly();
	}
}

public class Program
{
	public static void Main()
	{
		var manager = new CourseManager();

		var teacherA = new Teacher("Иван Иванов");
		var teacherB = new Teacher("Кирилл Кириллов");

		var online = new OnlineCourse("Алгоритмы и структуры данных", "Zoom");
		var offline = new OfflineCourse("Объектно-ориентированное программирование", "2304");

		manager.AddCourse(online);
		manager.AddCourse(offline);

		manager.AssignTeacher(online.Id, teacherA);
		manager.AssignTeacher(offline.Id, teacherB);

		var studentA = new Student("Сергей Сергеев");
		var studentB = new Student("Стас Стасов");

		manager.EnrollStudent(online.Id, studentA);
		manager.EnrollStudent(online.Id, studentB);
		manager.EnrollStudent(offline.Id, studentB);

		Console.WriteLine("Все курсы:");
		foreach (var course in manager.ListAll())
		{
			Console.WriteLine(course.Describe());
		}

		Console.WriteLine();
		Console.WriteLine($"Курсы преподавателя {teacherA.Name}:");
		foreach (var course in manager.GetCoursesByTeacher(teacherA.Id))
		{
			Console.WriteLine(course.Describe());
		}
	}
}
