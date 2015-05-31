using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO ;
using System.Threading ;
using System.Threading.Tasks ;
using NUnit.Framework ;

namespace alby.core.misc.test 
{
	public class MyTaskResult
	{
		public long					iterationsDone	= 0 ;
		public Dictionary<int, int> dic				= new Dictionary<int,int>() ;

		public MyTaskResult( int min, int max )
		{
			for ( int i = min ; i <= max ; i++ )
				dic[i] = 0 ;
		}
	}	

	[TestFixture]
	public class MyRandomTest
	{
		protected const string STOP_FILE = @"c:\temp\stop.txt" ;
		[SetUp]
		public void SetUp()
		{
		}

		[TearDown]
		public void TearDown()
		{
		}


		[Test]
		public void System6LottoNumbers_4Games()
		{
            int iterations = 6 * 4 ;
            int min = 1;
            int max = 45;

            MyRandom r = new MyRandom();
            for (int i = 1; i <= iterations; i++)
            {
                int n = r.GetSmallInt( min, max);
                Console.WriteLine("#{0}\t\t[{1}]", i, n ) ;
				
				if ( i % 6 == 0 )
					Console.WriteLine("####" ) ;
			}
		}

		[Test]
		public void System7LottoNumbers()
		{
            int iterations = 7 ;
            int min = 1;
            int max = 45;

            MyRandom r = new MyRandom();
            for (int i = 1; i <= iterations; i++)
            {
                int n = r.GetSmallInt( min, max);
                Console.WriteLine("#{0}\t\t[{1}]", i, n ) ;
			}
		}

		[Test]
		public void System8LottoNumbers()
		{
            int iterations = 8 ;
            int min = 1;
            int max = 45;

            MyRandom r = new MyRandom();
            for (int i = 1; i <= iterations; i++)
            {
                int n = r.GetSmallInt( min, max);
                Console.WriteLine("#{0}\t\t[{1}]", i, n ) ;
			}
		}

		// just how random are these numbers ?
		[Test, Explicit]
		public void TestRandomnessOfMyRandomClass()
		{
            long iterations = 100000L ; //500000000L;
            int  min = 1;
            int  max = 45;

            GC.Collect();
            GC.WaitForPendingFinalizers();

            Dictionary<int, int> dic = new Dictionary<int, int>();
			for ( int i = min ; i <= max ; i++ )
				dic[i] = 0 ;

            MyRandom r = new MyRandom();
			long iterationsDone = 0 ;

            for ( long i = 1; i <= iterations; i++ )
            {
				if ( File.Exists( STOP_FILE ) )
				{
					Console.WriteLine( "stop file detected" ) ;
					File.Delete( STOP_FILE ) ;
					break ;
				}

				iterationsDone++ ;
                int n = r.GetSmallInt( min, max);
                dic[n] = dic[n] + 1;
            }

			Console.WriteLine( "MY RANDOM - iterations done: {0}", iterationsDone ) ;
			if ( iterationsDone == 0 ) return ;

            double expectedPercentage = 1.0 / (double)(max - min + 1.0) * 100.0 ;

            foreach (int key in dic.Keys.OrderBy(x => x))
            {
                double percentage = (double) dic[key] / (double) iterationsDone * 100.0;
                Console.WriteLine("{0:00}\t\t{1}\t\t{2:00.0000}\t\t{3:00.0000}\t\t{4:00.0000}",
                    key,
                    dic[key],
                    percentage,
                    expectedPercentage,
                    Math.Abs( percentage - expectedPercentage)
                );     
            } 
		}

		// 5 000 000L
		// 1 thread =  72 sec 
		// 2 thread =  45 sec  
		// 3 thread =  37 sec 
		// 4 thread =  28 sec 
		// 5 thread =  27 sec 
		// 6 thread =  26 sec
		// 7 thread =  25 sec

		// 50 000 000L
		// 1 thread =   718 sec 
		// 2 thread =   462 sec  
		// 3 thread =   372 sec 
		// 4 thread =   281 sec 
		// 5 thread =   272 sec 

		// 1 000 000 000L
		// 4 thread =  5 788 sec
		 
		[Test, Explicit]
		public void TestRandomnessOfMyRandomClass_MT()
		{
            GC.Collect();
            GC.WaitForPendingFinalizers();

            long iterations = 1000000000L ;
			int  threads = 4 ;  
            int  min = 1  ;
            int  max = 45 ;
			bool abort = false ;

			long block = iterations / threads ;

			List< Task< MyTaskResult > > tasks = new List< Task< MyTaskResult > >() ;
			MyTaskResult sum = new MyTaskResult( 1, 45 ) ;
			ManualResetEvent mre = new ManualResetEvent( false ) ;

			///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

			Console.WriteLine( "MAIN: begin, tid = {0}, threads = {1}", Thread.CurrentThread.ManagedThreadId, threads ) ;

			///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

			// abort task 0
			Task taskListener = new Task
			( 
				( object taskNo ) => 
				{ 
					var tid = Thread.CurrentThread.ManagedThreadId ;

					Console.WriteLine( "\tLISTEN THREAD: START-HOLD, waiting for signal: task {0}, tid = {1}", taskNo, tid ) ; 
					mre.WaitOne() ;
					Console.WriteLine( "\tLISTEN THREAD: START-GO, go signal received: task {0}, tid = {1}", taskNo, tid ) ; 

					while ( true )
					{
						if ( abort ) 
						{
							Console.WriteLine( "\tLISTEN THREAD FINISH: ABORT, task {0}, tid = {1}", taskNo, tid ) ;
							return ;
						}
						if ( File.Exists( STOP_FILE ) )  
						{
							abort = true ;
							Console.WriteLine( "\tLISTEN THREAD FINISH: stop file detected, aborting: task {0}, tid = {1}", taskNo, tid ) ;
							return ;
						}
						Thread.Sleep( 500 ) ;
					}

				}, 0, TaskCreationOptions.LongRunning ) ;


			///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

			// calculation tasks 1..threads
			for ( int task = 1 ; task <= threads ; task++ )
				tasks.Add( new Task< MyTaskResult >
				( 
					( object taskNo ) => 
					{ 
						var tid = Thread.CurrentThread.ManagedThreadId ;

						Console.WriteLine( "\tTHREAD: START-HOLD, waiting for signal: task {0}, tid = {1}", taskNo, tid ) ; 
						mre.WaitOne() ;
						Console.WriteLine( "\tTHREAD: START-GO, go signal received: task {0}, tid = {1}", taskNo, tid ) ; 

						MyTaskResult mtr = new MyTaskResult( min, max ) ;
			            MyRandom random = new MyRandom();

						for ( long i = 1; i <= block; i++ )
						{
							if ( abort ) 
							{
								Console.WriteLine( "\tTHREAD: ABORT, task {0}, tid = {1}", taskNo, tid ) ;
								break ;
							}

							int n = random.GetSmallInt( min, max );
							mtr.dic[n] = mtr.dic[n] + 1;

							mtr.iterationsDone++ ;
						}

						Console.WriteLine( "\tTHREAD: FINISH, task {0}, tid = {1}", taskNo, tid ) ; 
						return mtr ;

					}, task, TaskCreationOptions.LongRunning ) 
				) ; 

			///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

			File.Delete( STOP_FILE ) ;

			Console.WriteLine( "MAIN: threads INIT, tid = {0}, threads = {1}", Thread.CurrentThread.ManagedThreadId, threads ) ;
			taskListener.Start() ;
			while ( true ) 
			{
				if ( taskListener.Status == TaskStatus.Running ) break ;
				Thread.Sleep( 0 ) ;
			}

			tasks.ForEach( t => t.Start() ) ;
			while ( true ) 
			{
				bool allStarted = true ;
				tasks.ForEach( t => 
					{ 
						if ( taskListener.Status != TaskStatus.Running )
							allStarted = false ;
				    } ) ;

				if ( allStarted ) break ;
				Thread.Sleep( 0 ) ;
			}

			Console.WriteLine( "MAIN: threads START-HOLD, holding for 10 secs, tid = {0}, threads = {1}", Thread.CurrentThread.ManagedThreadId, threads ) ;
			Thread.Sleep( 10000 ) ;

			Console.WriteLine( "MAIN: threads START-RELEASE, go signal sent, tid = {0}, threads = {1}", Thread.CurrentThread.ManagedThreadId, threads ) ;
			mre.Set() ;

			Console.WriteLine( "MAIN: threads WIP waiting to finish, tid = {0}, threads = {1}", Thread.CurrentThread.ManagedThreadId, threads ) ;
			tasks.ForEach( t => t.Wait() ) ;

			abort = true ;
			taskListener.Wait() ;
			Console.WriteLine( "MAIN: threads FINISHed, tid = {0}\n--------------", Thread.CurrentThread.ManagedThreadId ) ; 

			File.Delete( STOP_FILE ) ;

			///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

			List< MyTaskResult > results = new List< MyTaskResult > () ;
			tasks.ForEach( t => {
									if ( t != null ) 
										 results.Add( t.Result ) ;
								} ) ;

			results.ForEach( r =>
			{
				foreach( int key in r.dic.Keys )
					sum.dic[ key ] = sum.dic[key] + r.dic[key] ;

				sum.iterationsDone += r.iterationsDone ;
			} ) ;

			Console.WriteLine( "iterations done: {0}", sum.iterationsDone ) ; 
			if ( sum.iterationsDone == 0 ) return ;

            double expectedPercentage = 1.0 / (double)(max - min + 1.0) * 100.0 ;

            foreach ( int key in sum.dic.Keys.OrderBy(x => x))
            {
                double percentage = (double) sum.dic[key] / (double) sum.iterationsDone * 100.0;
                Console.WriteLine("{0:00}\t\t{1}\t\t{2:00.0000}\t\t{3:00.0000}\t\t{4:00.0000}",
                    key,
                    sum.dic[key],
                    percentage,
                    expectedPercentage,
                    Math.Abs( percentage - expectedPercentage)
                ) ;     
            } 
		}


		[Test, Explicit]
		public void TestRandomnessOfDotNetRandomClass()
		{
            long iterations = 500000000L;
            int  min = 1;
            int  max = 45;

            GC.Collect();
            GC.WaitForPendingFinalizers();

            Dictionary<int, int> dic = new Dictionary<int, int>();
			for ( int i = min ; i <= max ; i++ )
				dic[i] = 0 ;

            Random r = new Random();
			long iterationsDone = 0 ;

            for ( long i = 1; i <= iterations; i++ )
            {
				if ( File.Exists( STOP_FILE ) )
				{
					Console.WriteLine( "stop file detected" ) ;
					File.Delete( STOP_FILE ) ;
					break ;
				}

				iterationsDone++ ;
                int n = r.Next(  min, max+1 ) ;

                dic[n] = dic[n] + 1;
            }

			Console.WriteLine( ".NET RANDOM -iterations done: {0}", iterationsDone ) ;
			if ( iterationsDone == 0 ) return ;

            double expectedPercentage = 1.0 / (double)(max - min + 1.0) * 100.0 ;

            foreach (int key in dic.Keys.OrderBy(x => x))
            {
                double percentage = (double) dic[key] / (double) iterationsDone * 100.0;
                Console.WriteLine("{0:00}\t\t{1}\t\t{2:00.0000}\t\t{3:00.0000}\t\t{4:00.0000}",
                    key,
                    dic[key],
                    percentage,
                    expectedPercentage,
                    Math.Abs( percentage - expectedPercentage)
                );     
            } 


		}

	} // end class
}


/*

*
* MyRandom class:
*
------ Test started: Assembly: alby.core.test.dll ------

MAIN: begin, tid = 8, threads = 4
MAIN: threads INIT, tid = 8, threads = 4
	LISTEN THREAD: START-HOLD, waiting for signal: task 0, tid = 9
	THREAD: START-HOLD, waiting for signal: task 1, tid = 10
	THREAD: START-HOLD, waiting for signal: task 3, tid = 12
	THREAD: START-HOLD, waiting for signal: task 4, tid = 13
	THREAD: START-HOLD, waiting for signal: task 2, tid = 11
MAIN: threads START-HOLD, holding for 10 secs, tid = 8, threads = 4
MAIN: threads START-RELEASE, go signal sent, tid = 8, threads = 4
	THREAD: START-GO, go signal received: task 2, tid = 11
MAIN: threads WIP waiting to finish, tid = 8, threads = 4
	THREAD: START-GO, go signal received: task 4, tid = 13
	THREAD: START-GO, go signal received: task 3, tid = 12
	THREAD: START-GO, go signal received: task 1, tid = 10
	LISTEN THREAD: START-GO, go signal received: task 0, tid = 9
	THREAD: FINISH, task 3, tid = 12
	THREAD: FINISH, task 2, tid = 11
	THREAD: FINISH, task 4, tid = 13
	THREAD: FINISH, task 1, tid = 10
	LISTEN THREAD FINISH: ABORT, task 0, tid = 9
MAIN: threads FINISHed, tid = 8
--------------
iterations done: 1 000 000 000
01		22218246		02.2218		02.2222		00.0004
02		22221028		02.2221		02.2222		00.0001
03		22218713		02.2219		02.2222		00.0004
04		22220074		02.2220		02.2222		00.0002
05		22227983		02.2228		02.2222		00.0006
06		22220117		02.2220		02.2222		00.0002
07		22227763		02.2228		02.2222		00.0006
08		22220926		02.2221		02.2222		00.0001
09		22225089		02.2225		02.2222		00.0003
10		22226524		02.2227		02.2222		00.0004
11		22220400		02.2220		02.2222		00.0002
12		22223114		02.2223		02.2222		00.0001
13		22215227		02.2215		02.2222		00.0007
14		22225317		02.2225		02.2222		00.0003
15		22216623		02.2217		02.2222		00.0006
16		22221528		02.2222		02.2222		00.0001
17		22231780		02.2232		02.2222		00.0010
18		22228980		02.2229		02.2222		00.0007
19		22216786		02.2217		02.2222		00.0005
20		22228959		02.2229		02.2222		00.0007
21		22214190		02.2214		02.2222		00.0008
22		22220009		02.2220		02.2222		00.0002
23		22214423		02.2214		02.2222		00.0008
24		22226044		02.2226		02.2222		00.0004
25		22223111		02.2223		02.2222		00.0001
26		22222396		02.2222		02.2222		00.0000
27		22222779		02.2223		02.2222		00.0001
28		22221964		02.2222		02.2222		00.0000
29		22230872		02.2231		02.2222		00.0009
30		22222263		02.2222		02.2222		00.0000
31		22224342		02.2224		02.2222		00.0002
32		22230468		02.2230		02.2222		00.0008
33		22222426		02.2222		02.2222		00.0000
34		22220606		02.2221		02.2222		00.0002
35		22218708		02.2219		02.2222		00.0004
36		22227313		02.2227		02.2222		00.0005
37		22221277		02.2221		02.2222		00.0001
38		22222306		02.2222		02.2222		00.0000
39		22216367		02.2216		02.2222		00.0006
40		22217098		02.2217		02.2222		00.0005
41		22216859		02.2217		02.2222		00.0005
42		22223535		02.2224		02.2222		00.0001
43		22228095		02.2228		02.2222		00.0006
44		22223385		02.2223		02.2222		00.0001
45		22213987		02.2214		02.2222		00.0008

1 passed, 0 failed, 0 skipped, took 5788.86 seconds (NUnit 2.5.5).



 * 
 * 
 * 
.NET RANDOM -iterations done: 500000000
 * 
01		11111915		02.2224		02.2222		00.0002
02		11113972		02.2228		02.2222		00.0006
03		11105557		02.2211		02.2222		00.0011
04		11106821		02.2214		02.2222		00.0009
05		11106446		02.2213		02.2222		00.0009
06		11113197		02.2226		02.2222		00.0004
07		11113529		02.2227		02.2222		00.0005
08		11108339		02.2217		02.2222		00.0006
09		11111318		02.2223		02.2222		00.0000
10		11112942		02.2226		02.2222		00.0004
11		11106426		02.2213		02.2222		00.0009
12		11114199		02.2228		02.2222		00.0006
13		11112327		02.2225		02.2222		00.0002
14		11111893		02.2224		02.2222		00.0002
15		11111067		02.2222		02.2222		00.0000
16		11108454		02.2217		02.2222		00.0005
17		11110688		02.2221		02.2222		00.0001
18		11113292		02.2227		02.2222		00.0004
19		11112234		02.2224		02.2222		00.0002
20		11109713		02.2219		02.2222		00.0003
21		11106794		02.2214		02.2222		00.0009
22		11107772		02.2216		02.2222		00.0007
23		11116091		02.2232		02.2222		00.0010
24		11118032		02.2236		02.2222		00.0014
25		11117794		02.2236		02.2222		00.0013
26		11108163		02.2216		02.2222		00.0006
27		11115803		02.2232		02.2222		00.0009
28		11110396		02.2221		02.2222		00.0001
29		11107143		02.2214		02.2222		00.0008
30		11111167		02.2222		02.2222		00.0000
31		11110056		02.2220		02.2222		00.0002
32		11115133		02.2230		02.2222		00.0008
33		11109008		02.2218		02.2222		00.0004
34		11110917		02.2222		02.2222		00.0000
35		11113534		02.2227		02.2222		00.0005
36		11110837		02.2222		02.2222		00.0001
37		11113533		02.2227		02.2222		00.0005
38		11110494		02.2221		02.2222		00.0001
39		11109765		02.2220		02.2222		00.0003
40		11107929		02.2216		02.2222		00.0006
41		11109399		02.2219		02.2222		00.0003
42		11110780		02.2222		02.2222		00.0001
43		11110609		02.2221		02.2222		00.0001
44		11114528		02.2229		02.2222		00.0007
45		11109994		02.2220		02.2222		00.0002

1 passed, 0 failed, 0 skipped, took 6476.48 seconds (NUnit 2.5.5).

 */ 
