﻿using System.Collections.Generic;
using System.Linq;

namespace Lab2
{
    public class Elevator
    {
        private ElevatorState _state;

        public string StringState;

        public Elevator()
        {
            CurrentFloor = 1;
            _state = ElevatorState.Idle;
            StringState = $"Лифт стоит на {CurrentFloor} этаже";
            Buttons = new List<int>();
            UpButtons = new List<int>();
            DownButtons = new List<int>();
        }

        public ElevatorState State
        {
            get => _state;
            private set
            {
                _state = value;
                Program.OnStateChanged();
            }
        }

        public int CurrentFloor { get; private set; }
        public List<int> Buttons { get; }
        public List<int> UpButtons { get; }
        public List<int> DownButtons { get; }

        private void StateIdle()
        {
            switch (State)
            {
                case ElevatorState.Вoarding:
                    StringState = "Лифт закрыл двери";
                    State = ElevatorState.Idle;
                    break;
                case ElevatorState.Idle:
                    if (Buttons.Count == 0)
                        StringState = $"Лифт остановился на {CurrentFloor} этаже";
                    else
                        MoveTo(Buttons.First());
                    break;
            }
        }

        private void StateBoarding()
        {
            switch (State)
            {
                case ElevatorState.MovingUp:
                    UpButtons.Remove(CurrentFloor);
                    break;
                case ElevatorState.MovingDown:
                    UpButtons.Remove(CurrentFloor);
                    break;
            }
            StringState = "Лифт открыл двери";
            State = ElevatorState.Вoarding;
            Buttons.Remove(CurrentFloor);
            StateIdle();
        }

        private void StateMovingUp()
        {
            CurrentFloor++;
            StringState = $"Лифт поднялся на {CurrentFloor} этаж";
            State = ElevatorState.MovingUp;
            if (Buttons.Contains(CurrentFloor) || UpButtons.Contains(CurrentFloor))
                StateBoarding();
        }

        private void StateMovingUp(int floor)
        {
            while (CurrentFloor < floor)
                StateMovingUp();
        }

        private void StateMovingDown()
        {
            CurrentFloor--;
            StringState = $"Лифт опустился на {CurrentFloor} этаж";
            State = ElevatorState.MovingUp;
            if (Buttons.Contains(CurrentFloor) || DownButtons.Contains(CurrentFloor))
                StateBoarding();
        }

        private void StateMovingDown(int floor)
        {
            while (CurrentFloor > floor)
                StateMovingDown();
        }

        public void PressButton(int floor)
        {
            Buttons.Add(floor);
        }

        public void MoveTo(int floor)
        {
            if (CurrentFloor > floor)
                StateMovingDown(floor);
            else if (CurrentFloor < floor)
                StateMovingUp(floor);
            else
                StateBoarding();
            StateIdle();
        }
    }
}