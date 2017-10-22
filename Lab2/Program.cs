using System;
using System.Collections.Generic;
using System.Linq;

namespace Lab2
{
    internal static class Program
    {
        private static int _time;
        private static readonly Queue<Passenger> _futurePassengers = new Queue<Passenger>();
        private static readonly List<Passenger> _presentPassengers = new List<Passenger>();
        private static Elevator elevator;

        private static void Main(string[] args)
        {
            //var path = "";
            //var data = File.ReadAllLines(path).Skip(1);
            var ss = new[]
            {
                "1 1 3",
                "8 3 9",
                "11 7 1"
            };
            foreach (var s in ss)
                _futurePassengers.Enqueue(new Passenger(s.Split().Select(int.Parse)));

            Console.WriteLine("Time State");
            elevator = new Elevator();
            OnStateChanged();

            while (_futurePassengers.Count != 0 || _presentPassengers.Count != 0)
                if (_futurePassengers.Count != 0 && _futurePassengers.Peek()?.Time <= _time)
                {
                    var pass = _futurePassengers.Dequeue();
                    _presentPassengers.Add(pass);
                    if (pass.DestFloor - pass.StartFloor > 0)
                        elevator.UpButtons.Add(pass.StartFloor);
                    else
                        elevator.DownButtons.Add(pass.StartFloor);
                    elevator.MoveTo(pass.StartFloor);
                }
                else
                {
                    Console.WriteLine($"{_time,4} Лифт простаивает");
                    _time++;
                }
            Console.WriteLine("Конец программы");
            Console.Read();
        }

        public static void OnStateChanged()
        {
            Console.WriteLine($"{_time,4} {elevator.StringState}");
            _time++;
            if (elevator.State == ElevatorState.Вoarding)
                for (var i = 0; i < _presentPassengers.Count; i++)
                {
                    var pass = _presentPassengers.ElementAt(i);
                    if (elevator.CurrentFloor == pass.StartFloor)
                        elevator.PressButton(pass.DestFloor);
                    if (elevator.CurrentFloor == pass.DestFloor)
                        _presentPassengers.RemoveAt(i);
                }
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