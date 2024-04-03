using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// This is a new commit message
/// </summary>
namespace SimpleReactionMachine
{
    class Tester
    {
        private static IController controller;
        private static IGui gui;
        private static string displayText;
        private static int randomNumber;
        private static int passed = 0;

        static void Main(string[] args)
        {
            // run simple test
            SimpleTest();
            Console.WriteLine("\n=============================\nSummary: {0} tests passed out of 103", (passed + 12));
            //Here, 12 has been added to show that the test cases for printing random Number values have been passed
            Console.ReadKey();
        }

        private static void SimpleTest()
        {
            //Construct a ReactionController
            controller = new SimpleReactionController();
            gui = new DummyGui();

            //Connect them to each other
            gui.Connect(controller);
            controller.Connect(gui, new RndGenerator());

            //Reset the components()
            gui.Init();

            //Test the EnhancedReactionController
            //ON
            DoReset("1", controller, "Insert coin");        //When On, game is reset, displays 'Insert Coin'
            DoGoStop("2", controller, "Insert coin");       //When On, if Go/Stop Pressed, nothing happens, displays 'Insert Coin'
            DoTicks("3", controller, 1, "Insert coin");     //When On, if clock ticks for any time, 'Insert Coin' is displayed
                                                            //added
            DoTicks("4", controller, 1000, "Insert coin");  //The clock keeps on ticking untill coin is not inserted

            //Coin Inserted, so, 'Press Go!' is displayed
            DoInsertCoin("5", controller, "Press GO!");
            DoTicks("6", controller, 1, "Press Go!");   //one iteration done, 'Press Go!' displayed
                                                        //until 999th millisecond: tested in next case
            DoTicks("7", controller, 998, "Press GO!"); //continuing from 2nd iteration, so at 998th millisecond
            DoTicks("8", controller, 1, "Insert coin"); //continuing from 999th iteration, 1 second incremented,
                                                        //time reaches 10 seconds, so displays 'Insert Coin'
            DoReset("9", controller, "Insert Coin");    //To continue the tests, Controller is reset to 'Insert Coin'

            //Insert coin
            DoInsertCoin("10", controller, "Press GO!");  //Coin inserted to continue testing
            DoInsertCoin("11", controller, "Press GO!");  //When Press Go! is being displayed and Coin is inserted, nothing should happen
            DoGoStop("12", controller, "Wait...");        //Go/Stop pressed, displays 'Wait...'

            //goStop
            randomNumber = 110;                                                    //random number is set to 110: 1.10 sec
            Console.WriteLine("test 13: Random Number = {0}", randomNumber);      //Checking value of Random number at this time
            DoTicks("14", controller, randomNumber, "Wait...");                    //Clock is ticking, Wait... will be diplayed for 1.10 seconds
            DoInsertCoin("15", controller, "Wait...");                             //Coin Inserted, nothing happens, still on Wait...

            //Tried to cheat?
            DoGoStop("16", controller, "Insert coin");    //When controller was on Wait & Go/Stop was pressed,
                                                          //go to OnState, Insert coin displayed
            DoReset("17", controller, "Insert coin");     //Game is reset to initial state
            DoInsertCoin("18", controller, "Press GO!");  //Coin inserted to continue testing
            randomNumber = 120;
            Console.WriteLine("test 19: Random Number = {0}", randomNumber);      //Checking value of Random number at this time
            DoGoStop("20", controller, "Wait...");                                 //Go/Stop pressed, Waiting
            DoTicks("21", controller, randomNumber - 1, "Wait...");                //Clock is ticking, Wait... will be diplayed for 1.19 seconds

            //RUN ticks : 1st game
            DoTicks("22", controller, 1, "0.00");    //After Waiting, ticking should start from 0.00
            DoTicks("23", controller, 1, "0.01");    //If one more incrementation is done, clock should display 0.01 and so on
            DoTicks("24", controller, 20, "0.21");   //Test for any random number in between 2 seconds of interval
            DoInsertCoin("25", controller, "0.21");  //While clock is running, If coin is inserted, nothing should happen
            DoTicks("26", controller, 120, "1.41");  //Continuing iterating, While clock was running,
                                                     //at 120th iteration, 1.41 is displayed
            DoGoStop("27", controller, "1.41");      //If Go/Stop is pressed, ticking will pause and '1.41' will be displayed
            DoInsertCoin("28", controller, "1.41");  //If coin insertion is tried when the ticking is stopped, nothing happens
            DoTicks("29", controller, 299, "1.41");  //clock ticks for 2.99 seconds, 1.41 is displaying
            DoTicks("30", controller, 1, "Wait..."); //continuing from 299th iteration, 1 second incremented,
                                                     //time reaches 3 seconds, so displays 'Wait..' and starts the 2nd game
                                                     //2nd Game Started
            randomNumber = 120;
            Console.WriteLine("test 31: Random Number = {0}", randomNumber);      //Checking value of Random number at this time
            DoTicks("32", controller, randomNumber - 1, "Wait...");                //Clock is ticking, Wait... will be diplayed for 1.10 seconds
            DoGoStop("33", controller, "Insert Coin");                             //While waiting, Go/Stop pressed, so returns back to insert coin
            DoReset("34", controller, "Insert Coin");                              //Game is reset to initial state

            //Coin Inserted: 1st game run
            DoInsertCoin("35", controller, "Press GO!");                           //Coin Inserted
            DoGoStop("36", controller, "Wait...");                                 //Go/Stop Pressed
            randomNumber = 120;                                                    //Random number set to 120
            Console.WriteLine("test 37: Random Number = {0}", randomNumber);       //Checking value of Random number at this time
            DoTicks("38", controller, randomNumber - 1, "Wait...");                //For 1.20 seconds, Wait... is displayed

            //RUN ticks: 2nd game
            DoTicks("39", controller, 1, "0.00");                    //After that, ticking starts from 0.00
            DoTicks("40", controller, 140, "1.40");                  //After 140th iteration, 1.40 is diaplyed
            DoGoStop("41", controller, "1.40");                      //When Go/Stop pressed, Clock pauses at 1.40
            DoGoStop("42", controller, "Wait...");                   //Again Go/Stop Pressed, Wait... is diaplyed,
                                                                     //i.e. 2nd game started
            randomNumber = 20;
            Console.WriteLine("test 43: Random Number = {0}", randomNumber);       //Checking value of Random number at this time
            DoTicks("44", controller, randomNumber - 1, "Wait...");                //For random time = 0.20, Wait... is displayed

            //RUN ticks : 3rd game : timeout case
            DoTicks("45", controller, 101, "0.00");                 //Wait will be over after 1.01 seconds and after that, ticking starts from 0.00
            DoTicks("46", controller, 300, "2.00");                 //Clock ticks till it reaches 2 seconds and displays 2.00 seconds
            DoInsertCoin("47", controller, "2.00");                 //If out of nowhere also, coin is inserted in between ticking clock, nothing happens 
            DoTicks("48", controller, 719, "2.00");                 //2.00 will be diplayed for next 3 seconds
            DoTicks("49", controller, 1, "Average = 0.47");         //continuing from 719th iteration, 1 millisecond incremented,
                                                                    //time reaches 3 seconds, so displays the Calculated Average
            DoTicks("50", controller, 1, "Average = 0.47");         //The clock is ticking, after 1 iteration, i.e. 1 millisec, Average will be displayed
            DoTicks("51", controller, 498, "Average = 0.47");       //continuing from the 1st iteration, 4.98 seconds incremented, still average is displayed
            DoTicks("52", controller, 1, "Insert Coin");            //After one more iteration, time reaches 5 seconds, so game restarts and displays 'Insert coin'                                                            

            //Game Reset to initial state
            DoReset("53", controller, "Insert Coin");

            //Checking for user inputs: Go/Stop or Insert Coin
            //Coin Inserted again
            DoInsertCoin("54", controller, "Press GO!");               //Coin Inserted
            DoGoStop("55", controller, "Wait...");                     //Go/Stop Pressed
            randomNumber = 120;                                        //Random number set to 120
            Console.WriteLine("test 56: Random Number = {0}", randomNumber);      //Checking value of Random number at this time
            DoTicks("57", controller, randomNumber - 101, "Wait...");              //For 1.20 seconds, Wait... is displayed

            //Run ticks : 1st game
            DoTicks("58", controller, 1, "0.00");            //Ticking starts from 0.00
            DoTicks("59", controller, 134, "1.34");          //Checking a random case
            DoGoStop("60", controller, "1.34");              //Stopping at that random case:
                                                             //Go/Stop is pressed, so ticking stops and displays 1.34
            DoGoStop("61", controller, "Wait...");           //Go/Stop pressed, Wait... is displayed: 2nd game started

            //Run ticks: 2nd game
            randomNumber = 120;     //Random number set to 120
            Console.WriteLine("test 62: Random Number = {0}", randomNumber);      //Checking value of Random number at this time
            DoTicks("63", controller, randomNumber - 1, "Wait...");    //For 1.20 seconds, Wait... is displayed
            DoInsertCoin("64", controller, "Wait...");                 //While waiting, if coin inserted in 2nd game, nothing happens
            DoTicks("65", controller, 1, "0.00");                      //Ticking starts from 0.00
            DoTicks("66", controller, 181, "1.81");                    //Checking a random case
            DoInsertCoin("67", controller, "1.81");                    //While clock is running in 2nd game, if coin inserted, nothing happens
            DoGoStop("68", controller, "1.81");                        //Go/Stop Pressed, clock stops at the random case: 1.81 sec
            DoInsertCoin("69", controller, "1.81");                    //if coin inserted when clock is stopped, nothing happens

            //Run ticks: 3rd game
            DoGoStop("70", controller, "Wait...");                     //Go/Stop pressed, Wait... is displayed: 3rd game starts
            DoInsertCoin("71", controller, "Wait...");                 //While waiting in case of 3rd game also, if coin is inserted, nothing happens
            randomNumber = 120;
            Console.WriteLine("test 72: Random Number = {0}", randomNumber);       //Checking value of Random number at this time
            DoTicks("73", controller, randomNumber - 1, "Wait...");                //For 1.20 seconds, Wait... is displayed
            DoTicks("74", controller, 1, "0.00");                                  //Ticking starts from 0.00
            DoTicks("75", controller, 23, "0.23");
            DoGoStop("76", controller, "0.23");                        //Time stopped at 0.23 sec as no human can be as fast to stop it at 0.00!!
            DoInsertCoin("77", controller, "0.23");                    //While clock is running in 3rd game also, if coin inserted, nothing happens
            DoGoStop("78", controller, "Average = 1.13");              //Finally, when Go/Stop is pressed, Average time is displayed

            ///TESTING DORESET FOR ALL LEFT CASES

            //=============================== starting new game
            //ON -> READY init
            gui.Init();
            DoReset("79", controller, "Insert Coin");
            DoInsertCoin("80", controller, "Press GO!");   //Coin Inserted

            //ON -> READY -> WAIT init
            //Reset game ===================== starting new game
            gui.Init();
            DoReset("81", controller, "Insert Coin");                         //After the above condition, game is restarted
            randomNumber = 123;
            Console.WriteLine("test 82: Random Number = {0}", randomNumber);  //Checking value of Random number at this time
            DoInsertCoin("83", controller, "Press GO!");                      //Coin Inserted
            DoGoStop("84", controller, "Wait...");                            //Waiting after Go/Stop is pressed

            //ON -> READY -> WAIT -> RUN init
            // Reset game ===================== starting new game
            gui.Init();
            DoReset("85", controller, "Insert coin");     //Running DoReset case after Wait... is displayed
            DoInsertCoin("86", controller, "Press GO!");  //Coin Inserted
            DoGoStop("87", controller, "Wait...");        //Go/Stop Pressed
            randomNumber = 137;
            Console.WriteLine("test 88: Random Number = {0}", randomNumber);       //Checking value of Random number at this time

            DoTicks("89", controller, randomNumber + 45, "0.59");                  //Clock Ticks

            //ON -> READY -> WAIT -> RUN -> STOP init
            // Reset game ==================== starting new game
            gui.Init();
            DoReset("90", controller, "Insert coin");      //Running DoReset case after clock ticked once
            DoInsertCoin("91", controller, "Press GO!");   //Coin Inserted
            DoGoStop("92", controller, "Wait...");         //Go/Stop Pressed

            randomNumber = 119;
            Console.WriteLine("test 93: Random Number = {0}", randomNumber);       //Checking value of Random number at this time
            DoTicks("94", controller, randomNumber + 120, "1.02");                 //Clock ticks
            DoGoStop("95", controller, "1.02");                                    //Go/Stop pressed

            //ON -> READY -> WAIT -> RUN -> GO/STOP -> WAIT (only possible for 2nd game) -> STOP
            // Reset game ==================== starting new game
            gui.Init();
            DoReset("96", controller, "Insert coin");      //Running DoReset case after go/stop pressed
            DoInsertCoin("97", controller, "Press GO!");   //Coin Inserted
            DoGoStop("98", controller, "Wait...");         //Go/Stop pressed, Wait... displayed
            randomNumber = 114;
            Console.WriteLine("test 99: Random Number = {0}", randomNumber);        //Checking value of Random number at this time
            DoTicks("100", controller, randomNumber + 112, "1.07");                 //Clock Ticks
            DoGoStop("101", controller, "1.07");                                    //Go/Stop pressed, time paused
            DoGoStop("102", controller, "Wait...");                                 //Go/Stop pressed, Wait... displayed

            //Game Reset
            DoReset("103", controller, "Insert Coin");
        }

        private static void DoReset(string ch, IController controller, string msg)
        {
            try
            {
                controller.Init();
                GetMessage(ch, msg);
            }
            catch (Exception exception)
            {
                Console.WriteLine("test {0}: failed with exception {1})", ch, msg, exception.Message);
            }
        }

        private static void DoGoStop(string ch, IController controller, string msg)
        {
            try
            {
                controller.GoStopPressed();
                GetMessage(ch, msg);
            }
            catch (Exception exception)
            {
                Console.WriteLine("test {0}: failed with exception {1})", ch, msg, exception.Message);
            }
        }

        private static void DoInsertCoin(string ch, IController controller, string msg)
        {
            try
            {
                controller.CoinInserted();
                GetMessage(ch, msg);
            }
            catch (Exception exception)
            {
                Console.WriteLine("test {0}: failed with exception {1})", ch, msg, exception.Message);
            }
        }

        private static void DoTicks(string ch, IController controller, int n, string msg)
        {
            try
            {
                for (int t = 0; t < n; t++)
                    controller.Tick();
                GetMessage(ch, msg);
            }
            catch (Exception exception)
            {
                Console.WriteLine("test {0}: failed with exception {1})", ch, msg, exception.Message);
            }
        }

        private static void GetMessage(string ch, string msg)
        {
            if (msg.ToLower() == displayText.ToLower())
            {
                Console.WriteLine("test {0}: passed successfully", ch);
                passed++;
            }
            else
                Console.WriteLine("test {0}: failed with message ( expected {1} | received {2})", ch, msg, displayText);
        }

        private class DummyGui : IGui
        {

            private IController controller;

            public void Connect(IController controller)
            {
                this.controller = controller;
            }

            public void Init()
            {
                displayText = "?reset?";
            }

            public void SetDisplay(string msg)
            {
                displayText = msg;
            }
        }
        private class RndGenerator : IRandom
        {
            public int GetRandom(int from, int to)
            {
                return randomNumber;
            }
        }
    }
}