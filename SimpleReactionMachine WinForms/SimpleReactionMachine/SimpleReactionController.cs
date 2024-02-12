using SimpleReactionMachine;

class SimpleReactionController : IController
{
    private int Ticks;
    private int Game;
    private double sum;
    private IGui Gui;
    private IRandom Rng;
    private ControllerState _Cstate;

    public void Connect(IGui gui, IRandom rng)
    {
        this.Gui = gui;
        this.Rng = rng;
        Init();
    }

    //Default case
    public void Init()
    {
        this._Cstate = new OnState(this);
    }

    //Initialising all the methods in the IController interface
    public void CoinInserted()
    {
        this._Cstate.CoinInserted();
    }

    public void GoStopPressed()
    {
        this._Cstate.GoStopPressed();
    }

    public void Tick()
    {
        this._Cstate.Tick();
    }

    //Definition of main ControllerState
    public abstract class ControllerState
    {
        public SimpleReactionController _controller;

        public ControllerState(SimpleReactionController controller)
        {
            this._controller = controller;
        }

        public abstract void CoinInserted();

        public abstract void GoStopPressed();

        public abstract void Tick();
    }

    public class OnState : ControllerState
    {
        public OnState(SimpleReactionController controller) : base(controller)
        {
            this._controller.Gui.SetDisplay("Insert coin");
            this._controller.Game = 0;
            this._controller.sum = 0;
        }

        //If coin inserted, control goes to ReadyState where "Press Go!" will be displayed
        public override void CoinInserted()
        {
            this._controller._Cstate = new ReadyState(this._controller);
        }

        //If GoStop is pressed nothing happens
        public override void GoStopPressed() { }

        //No ticking in this case
        public override void Tick() { }
    }

    public class ReadyState : ControllerState
    {
        //Press Go is diplayed on the screen after the coin is inserted
        public ReadyState(SimpleReactionController controller) : base(controller)
        {
            this._controller.Gui.SetDisplay("Press GO!");
            this._controller.Ticks = 0;
        }

        //if coin inserted, nothing happens
        public override void CoinInserted() { }

        //If GoStop is pressed, control goes to Wait state, where we need to wait for a random time between 3 sec
        public override void GoStopPressed()
        {
            this._controller._Cstate = new WaitState(this._controller);
        }

        //The clock ticks for 10 seconds if GoStop is not pressed and then returns back to the OnState
        public override void Tick()
        {
            this._controller.Ticks++;
            if (this._controller.Ticks == 1000)
            {
                this._controller._Cstate = new OnState(this._controller);
            }
        }
    }

    public class WaitState : ControllerState
    {
        private int _wait;

        //Wait... is diplayed for any random time between 1 to 2.5 seconds
        public WaitState(SimpleReactionController controller) : base(controller)
        {
            this._controller.Gui.SetDisplay("Wait...");
            this._controller.Ticks = 0;
            _wait = this._controller.Rng.GetRandom(100, 250);
        }

        //if coin inserted, nothing happens
        public override void CoinInserted() { }

        //While waiting, if GoStop is pressed, control goes back to Onstate, where we will have to restart the game by inserting the coin
        public override void GoStopPressed()
        {
            this._controller._Cstate = new OnState(this._controller);
        }

        //The clock ticks for the random time which will be chosen by the compiler to wait, and then, progresses to the RunState, where the timer will run
        public override void Tick()
        {
            this._controller.Ticks++;
            if (this._controller.Ticks == _wait)
            {
                this._controller._Cstate = new RunState(this._controller);
            }
        }
    }

    public class RunState : ControllerState
    {
        //The timer of 2 seconds runs after the Wait is over and the user will have to press GoStop in order to stop it.
        //That is the actual purpose of the Reaction Timer game 
        public RunState(SimpleReactionController controller) : base(controller)
        {
            this._controller.Ticks = 0;
            this._controller.Gui.SetDisplay("0.00");
        }

        //If coin is inserted, nothing happens
        public override void CoinInserted() { }

        //If GoStop is pressed, the clock stops where it is and then moves to the Display state to display the time at which it stopped
        public override void GoStopPressed()
        {
            this._controller.sum += this._controller.Ticks;
            this._controller._Cstate = new DisplayState(this._controller);
        }

        //While running, the clock will be ticking incrementing 10 milliseconds each time and displaying it in the format of 0.00 as instructed in the task sheet for 2 seconds.
        //If GoStop is not pressed between these 2 seconds, the clock will stop there itself for 3 seconds after moving to the Display state 
        public override void Tick()
        {
            this._controller.Ticks++;
            this._controller.Gui.SetDisplay((this._controller.Ticks / 100.0).ToString("0.00"));
            if (this._controller.Ticks == 200)
            {
                this._controller._Cstate = new DisplayState(this._controller);
            }
        }
    }

    public class DisplayState : ControllerState
    {
        //This state displays the current time of the ticking clock where it has been stopped
        public DisplayState(SimpleReactionController controller) : base(controller)
        {
            this._controller.Ticks = 0;
            this._controller.Game++;
        }

        //While displaying, if coin is inserted, nothing will happen
        public override void CoinInserted() { }

        //Out of the total 3 games, after and during the 1st and 2nd games, if GoStop is pressed within 2 seconds, the control moves to Wait state
        //While after the 3rd game is over, the control goes to the Average state
        public override void GoStopPressed()
        {
            if (this._controller.Game == 3)
            {
                this._controller._Cstate = new AverageState(this._controller);
            }
            else
            {
                this._controller._Cstate = new WaitState(this._controller);
            }
        }

        //When the 1st or 2nd games are being played, after the 2 seconds are over, the clock ticks and waits for 3 seconds for the user to respond
        //If the user does not respond, the control moves to Wait state automatically
        //While in case of 3rd game, after the 3 seconds are over, it automatically moves to Average state
        public override void Tick()
        {
            this._controller.Ticks++;
            if (this._controller.Ticks == 300 && this._controller.Game == 3)
            {
                this._controller._Cstate = new AverageState(this._controller);
            }
            if (this._controller.Ticks == 300 && this._controller.Game != 3)
            {
                this._controller._Cstate = new WaitState(this._controller);
            }
        }
    }

    public class AverageState : ControllerState
    {
        //This class calculates the average of user's reaction time
        //It will display the key word average and the calculate average in front of it in the form of 0.00.
        public AverageState(SimpleReactionController controller) : base(controller)
        {
            this._controller.Gui.SetDisplay("Average = " + (this._controller.sum / 300).ToString("0.00"));
            this._controller.Ticks = 0;
        }

        //While in Average state, if coin is inserted, nothing will happen
        public override void CoinInserted() { }

        //After the average is displayed, if GoStop is pressed, the control goes back to the initial OnState
        public override void GoStopPressed()
        {
            this._controller._Cstate = new OnState(this._controller);
        }

        //After the average is diplayed, if user does not presses GoStop, the clock waits (ticks) for 5 seconds and then moves back to OnState
        public override void Tick()
        {
            this._controller.Ticks++;
            if (this._controller.Ticks == 500)
            {
                this._controller._Cstate = new OnState(this._controller);
            }
        }
    }
}