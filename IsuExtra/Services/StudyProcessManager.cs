using System.Collections.Generic;
using System.Linq;
using Isu.Tools;
using IsuExtra.Entities;

namespace IsuExtra.Services
{
    public class StudyProcessManager
    {
        private const int DefaultStartTeacherId = 1;
        private readonly List<Teacher> _allTeachers;
        private readonly List<Room> _allRooms;
        private int _uniqId;

        public StudyProcessManager(int starterId = DefaultStartTeacherId)
        {
            _allRooms = new List<Room>();
            _allTeachers = new List<Teacher>();
            _uniqId = starterId;
        }

        public Teacher AddTeacher(string name)
        {
            var newTeacher = new Teacher(name, _uniqId++);
            _allTeachers.Add(newTeacher);
            return newTeacher;
        }

        public Room AddRoom(string number)
        {
            if (_allRooms.Any(room => room.Number == number))
            {
                throw new IsuException($"Room with number {number} already exists!");
            }

            var room = new Room(number);
            _allRooms.Add(room);
            return room;
        }

        public Teacher GetTeacher(int id)
        {
            Teacher teacher = _allTeachers.FirstOrDefault(teacher => teacher.Id == id);
            return teacher ?? throw new IsuException($"Teacher with id {id} doesn't exist");
        }

        public Teacher FindTeacher(string name)
        {
            return _allTeachers.FirstOrDefault(teacher => teacher.Name == name);
        }

        public Room FindRoom(string number)
        {
            return _allRooms.FirstOrDefault(room => room.Number == number);
        }
    }
}