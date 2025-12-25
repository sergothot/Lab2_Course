using Lab2_Course;
using Xunit;

namespace Lab2_Course.Tests;

public class CourseManagerTests
{
    [Fact]
    public void AddCourse_ShouldAppearInListAll()
    {
        var manager = new CourseManager();
        var course = new OnlineCourse("Тест", "Zoom");

        manager.AddCourse(course);

        Assert.Contains(course, manager.ListAll());
    }

    [Fact]
    public void AssignTeacher_ShouldReturnTrueAndSetTeacher()
    {
        var manager = new CourseManager();
        var course = new OnlineCourse("Тест", "Zoom");
        var teacher = new Teacher("Петр");
        manager.AddCourse(course);

        var result = manager.AssignTeacher(course.Id, teacher);

        Assert.True(result);
        Assert.Equal(teacher, course.Teacher);
    }

    [Fact]
    public void EnrollStudent_PreventsDuplicate()
    {
        var manager = new CourseManager();
        var course = new OfflineCourse("Тест", "101");
        var student = new Student("Игорь");
        manager.AddCourse(course);

        var first = manager.EnrollStudent(course.Id, student);
        var second = manager.EnrollStudent(course.Id, student);

        Assert.True(first);
        Assert.False(second);
        Assert.Single(course.Students);
    }

    [Fact]
    public void GetCoursesByTeacher_FiltersProperly()
    {
        var manager = new CourseManager();
        var teacherA = new Teacher("A");
        var teacherB = new Teacher("B");
        var courseA = new OnlineCourse("C1", "Zoom");
        var courseB = new OfflineCourse("C2", "201");
        manager.AddCourse(courseA);
        manager.AddCourse(courseB);
        manager.AssignTeacher(courseA.Id, teacherA);
        manager.AssignTeacher(courseB.Id, teacherB);

        var result = manager.GetCoursesByTeacher(teacherA.Id);

        Assert.Single(result);
        Assert.Contains(courseA, result);
        Assert.DoesNotContain(courseB, result);
    }

    [Fact]
    public void RemoveCourse_DeletesCourse()
    {
        var manager = new CourseManager();
        var course = new OnlineCourse("Удалить", "Zoom");
        manager.AddCourse(course);

        var removed = manager.RemoveCourse(course.Id);

        Assert.True(removed);
        Assert.DoesNotContain(course, manager.ListAll());
    }

    [Fact]
    public void UnenrollStudent_RemovesStudent()
    {
        var manager = new CourseManager();
        var course = new OnlineCourse("Тест", "Zoom");
        var student = new Student("Сережа");
        manager.AddCourse(course);
        manager.EnrollStudent(course.Id, student);

        var removed = manager.UnenrollStudent(course.Id, student.Id);

        Assert.True(removed);
        Assert.Empty(course.Students);
    }

    [Fact]
    public void Describe_ReturnsReadableSummary()
    {
        var course = new OnlineCourse("Сети", "Teams");
        var teacher = new Teacher("Даша");
        course.AssignTeacher(teacher);
        course.AddStudent(new Student("Аня"));
        course.AddStudent(new Student("Виктор"));

        var text = course.Describe();

        Assert.Contains("Онлайн", text);
        Assert.Contains("Сети", text);
        Assert.Contains("Даша", text);
        Assert.Contains("2", text);
    }
}
