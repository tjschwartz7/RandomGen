
#define ENTROPY
//#define PSEUDO

using System.Numerics;
using System.Runtime.CompilerServices;
using System.Threading;


class MainSource
{
    static void Main(string[] args)
    {
#if ENTROPY
        for(int i = 0; i < 1000; i++)
        {
            long rand = NanoEntropy.newEntropy();
            Console.WriteLine(String.Format("{0:X}", rand));
        }
#endif

#if PSEUDO
        Console.WriteLine("Pseudorandom numbers...\n\n\n");
        PseudoGen gen = new PseudoGen(65537);
        for (int i = 0; i < 10; i++)
        {
            long rand = gen.NewRandom();
            Console.WriteLine(String.Format("{0}", rand));
        }
#endif



    }
}

class NanoEntropy
{
    private static long rand;
    private static long seed;
    private static int maxTimeWaitPerIter;

    public static long newEntropy()
    {
        maxTimeWaitPerIter = 2;
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


        //This is just getting ridiculous...
        //Mod is here to prevent memory usage from getting... out of hand
        //The idea here is to introduce cache *incoherency* to 
        //make everything that much less predictable
        int lastPrime = SieveEratosthenes(Math.Abs((int)rand) % 1000000);

        rand = (rand * (long)DateTime.Now.Microsecond);
        //Xor it with the last prime we found...
        rand ^= lastPrime;
    }


    //This algorithm is designed to search for prime numbers up to some max number
    //It will print a list of all the primes it finds
    public static int SieveEratosthenes(int checkUpTo)
    {
        //This algorithm is pretty fast, but the fact it makes an array of checkUpTo bools is not great
        //Its also subject to many cache misses since it's not always dealing with adjacent integers
        //This algorithm cannot be used for checkUpTo sizes beyond the limits of our heap


        //Generate a list of integers from 2 to N
        //Use array indices as integer marker
        //Bool represents isPrime relation
        bool[] intIsNotPrime = new bool[checkUpTo];
        int p = 2, sqrtN = (int)(Math.Sqrt(checkUpTo)) + 1;

        while (p < sqrtN)
        {
            int iter = p;

            //Since we add first, we'll iterate up to checkUpTo - p
            while (iter < checkUpTo - p)
            {
                //iter+= p
                iter += p;
                //Mark off as NOT prime
                intIsNotPrime[iter] = true;
            }

            //We don't want to double count our current prime number - hence do_while
            do
            {
                p++;
            } while (intIsNotPrime[p]); //Iterate p until it's prime
        }

        //Return the last prime we found
        return p;
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
