
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Threading;

class MainSource
{
    static void Main(string[] args)
    {
        for(int i = 0; i < 10; i++)
        {
            long rand = NanoEntropy.newEntropy();
            Console.WriteLine(String.Format("{0:X}", rand));
        }

        Console.WriteLine("Pseudorandom numbers...\n\n\n");
        PseudoGen gen = new PseudoGen(65537);
        for (int i = 0; i < 1000; i++)
        {
            long rand = gen.NewRandom();
            Console.WriteLine(String.Format("{0}", rand));
        }



    }
}

class NanoEntropy
{
    private static long rand;
    private static long seed;
    private static int maxTimeWaitPerIter;

    public static long newEntropy()
    {
        maxTimeWaitPerIter = 3;
        rand = 1;
        seed = DateTime.Now.Ticks;
        Thread thread1 = new Thread(new ThreadStart(threadedNewEntropy));

        //Round one
        thread1.Start();
        thread1.Join();

        //Xor rand with the seed
        rand ^= seed;

        Thread thread2 = new Thread(new ThreadStart(threadedNewEntropy));
        //Round two
        thread2.Start();
        thread2.Join();

        //Xor rand with the seed
        rand ^= seed;


        Thread thread3 = new Thread(new ThreadStart(threadedNewEntropy));
        //Round three
        thread3.Start();
        thread3.Join();

        //Xor rand with the seed
        rand ^= seed;

        return rand;
    }


    static void threadedNewEntropy()
    {
        //Added level of entropy depending on how long you're willing to wait for it
        int moreRandom = (int)rand % maxTimeWaitPerIter;
        if (moreRandom < 0) moreRandom *= -1;
        Thread.Sleep(moreRandom);
        rand = (rand * (long)DateTime.Now.Microsecond);
    }


}

class PseudoGen
{
    private int seed;
    private int multiplier, adder;
    public PseudoGen(int seed) 
    { 
        this.seed = seed;

        //Nonnegative integers
        multiplier = 0x7AB53;
        adder = 0x1ABCEF;
    }
    public PseudoGen(int seed, int multiplier, int adder)
    {
        this.seed = seed;
        this.multiplier = multiplier;
        this.adder = adder;
    }

    /// <summary>
    /// Returns a pseudorandom number that is a function of an input seed.
    /// </summary>
    /// <returns>The pseudorandom number.</returns>
    public long NewRandom()
    {
        //How large do we allow our numbers to be
        int cap = int.MaxValue; //No limit on size in default case
        int rand = (seed * multiplier + adder) % cap;
        seed = rand;

        return rand;
    }

    /// <summary>
    /// Accepts a maxima as an argument and returns a pseudorandom number.
    /// </summary>
    /// <param name="cap">Our random numbers maxima.</param>
    /// <returns>The pseudorandom number.</returns>
    public long NewRandom(int cap)
    {

        int rand = (seed * multiplier + adder) % cap;
        seed = rand;

        return rand;
    }
}
