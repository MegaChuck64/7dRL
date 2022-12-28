
namespace Engine.Utilities;

public static class Random
{
    private static System.Random rand;


    public static int GetInt(int maxExclusive, int min = 0)
    {
        if (rand == null) rand = new System.Random();
        return rand.Next(min,maxExclusive);
    }

}