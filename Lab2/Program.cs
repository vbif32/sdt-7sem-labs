using System;
using System.Collections.Generic;
using System.Linq;

namespace Lab2
{
    internal static class Program
    {
        private static int _time;
        private static readonly List<Passenger> FuturePassengers = new List<Passenger>();
        private static readonly List<Passenger> PresentPassengers = new List<Passenger>();
        private static Elevator _elevator;

        private static void Main(string[] args)
        {
            //var path = "";
            //var data = File.ReadAllLines(path).Skip(1);
            var ss = new[]
            {
                "1 3 5",
                "2 4 6",
                "6 6 4",
                "7 7 9",
                "1 9 1",
                //"1 1 2",
                //"1 1 10"
            };
            foreach (var s in ss)
                FuturePassengers.Add(new Passenger(s.Split().Select(int.Parse)));

            Console.WriteLine("Time State");
            _elevator = new Elevator();
            OnStateChanged();

            while (FuturePassengers.Count != 0 || PresentPassengers.Count != 0)
            {
                CheckUpcomingPassengers();
                _elevator.Update();
            }
            Console.WriteLine("Конец программы");
            Console.Read();
        }

        public static void OnStateChanged()
        {
            Console.WriteLine($"{_time,4} {_elevator.StringState}");
            _time++;
            CheckUpcomingPassengers();
            if (_elevator.State != ElevatorState.Вoarding) return;
            for (var i = 0; i < PresentPassengers.Count; i++)
            {
                var pass = PresentPassengers.ElementAt(i);
                if (_elevator.CurrentFloor == pass.StartFloor)
                    _elevator.PressButton(pass.DestFloor);
            }
            PresentPassengers.RemoveAll(p => _elevator.CurrentFloor == p.DestFloor);
        }

        public static void CheckUpcomingPassengers()
        {
            if (FuturePassengers.Count == 0) return;
            var func = new Func<Passenger, bool>(p => p.Time <= _time);
            var predicate = new Predicate<Passenger>(func);
            var ps = FuturePassengers.Where(func);
            foreach (var p in ps)
            {
                PresentPassengers.Add(p);
                if (p.DestFloor - p.StartFloor > 0)
                    _elevator.PressUpButton(p.StartFloor);
                else
                    _elevator.PressDownButton(p.StartFloor);
            }
            FuturePassengers.RemoveAll(predicate);
        }

        private class Passenger
        {
            public Passenger(IEnumerable<int> data)
            {
                Time = data.ElementAt(0);
                StartFloor = data.ElementAt(1);
                DestFloor = data.ElementAt(2);
            }

            public int Time { get; }
            public int StartFloor { get; }
            public int DestFloor { get; }
        }
    }
}