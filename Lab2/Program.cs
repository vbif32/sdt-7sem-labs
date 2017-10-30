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
                "1 1 2",
                "1 1 10"
            };
            foreach (var s in ss)
                FuturePassengers.Add(new Passenger(s.Split().Select(int.Parse)));

            Console.WriteLine("Time State");
            _elevator = new Elevator();
            OnStateChanged();


            while (FuturePassengers.Count != 0 || PresentPassengers.Count != 0)
                if (FuturePassengers.Count != 0 && FuturePassengers.Any(p => p.Time <= _time))
                {
                    var passes = FuturePassengers.Where(p => p.Time <= _time);
                    PresentPassengers.AddRange(passes);
                    FuturePassengers.RemoveAll(p => p.Time <= _time);
                    foreach (var pass in passes)
                        if (pass.DestFloor - pass.StartFloor > 0)
                            _elevator.UpButtons.Add(pass.StartFloor);
                        else
                            _elevator.DownButtons.Add(pass.StartFloor);
                    _elevator.MoveTo(PresentPassengers.First().StartFloor);
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
            Console.WriteLine($"{_time,4} {_elevator.StringState}");
            _time++;
            if (_elevator.State == ElevatorState.Вoarding)
                for (var i = 0; i < PresentPassengers.Count; i++)
                {
                    var pass = PresentPassengers.ElementAt(i);
                    if (_elevator.CurrentFloor == pass.StartFloor)
                        _elevator.PressButton(pass.DestFloor);
                    if (_elevator.CurrentFloor == pass.DestFloor)
                        PresentPassengers.RemoveAt(i);
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