using System;
using Isu.Entities;
using Isu.Models;

namespace Isu.Services
{
    public class CourseManager
    {
        public Subject AddSubjectOnCourse(Course course, string name)
        {
            if (course == null)
            {
                throw new ArgumentNullException(
                    nameof(course),
                    $"{nameof(course)} can't be null!");
            }

            return course.AddSubject(name);
        }

        public void DeleteSubjectFromCourse(Course course, Subject subject)
        {
            if (course == null)
            {
                throw new ArgumentNullException(
                    nameof(course),
                    $"{nameof(course)} can't be null!");
            }

            course.DeleteSubject(subject);
        }

        public GroupStudyClass AddGroupStudyClass(
            Subject subject,
            TimeStamp timeStamp,
            Group group,
            Teacher teacher,
            Room room)
        {
            if (subject == null)
            {
                throw new ArgumentNullException(
                    nameof(subject),
                    $"{nameof(subject)} can't be null");
            }

            if (timeStamp == null)
            {
                throw new ArgumentNullException(
                    nameof(timeStamp),
                    $"{nameof(timeStamp)} can't be null");
            }

            if (teacher == null)
            {
                throw new ArgumentNullException(
                    nameof(teacher),
                    $"{nameof(teacher)} can't be null");
            }

            if (group == null)
            {
                throw new ArgumentNullException(
                    nameof(group),
                    $"{nameof(group)} can't be null");
            }

            if (room == null)
            {
                throw new ArgumentNullException(
                    nameof(room),
                    $"{nameof(room)} can't be null");
            }

            return subject.AddStudyClass(timeStamp, group, teacher, room);
        }

        public void DeleteStudyClass(Subject subject, GroupStudyClass groupStudyClass)
        {
            if (subject == null)
            {
                throw new ArgumentNullException(
                    nameof(subject),
                    $"{nameof(subject)} can't be null");
            }

            subject.DeleteStudyClass(groupStudyClass);
        }
    }
}