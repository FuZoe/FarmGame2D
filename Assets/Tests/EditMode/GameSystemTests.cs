using NUnit.Framework;
using UnityEngine;

public class GameSystemTests
{
    [Test]
    public void FarmPlotStartsUntilled()
    {
        FarmPlotState plot = new FarmPlotState(2, 3);
        Assert.IsFalse(plot.tilled);
        Assert.IsFalse(plot.planted);
        Assert.IsFalse(plot.ready);
    }

    [Test]
    public void ClockAdvancesToNextDay()
    {
        GameObject gameObject = new GameObject("ClockTest");
        GameClock clock = gameObject.AddComponent<GameClock>();
        clock.SetTime(1, 23 * 60 + 59);
        clock.AdvanceMinutes(1);
        Assert.AreEqual(2, clock.Day);
        Assert.AreEqual(0, clock.MinuteOfDay);
        Object.DestroyImmediate(gameObject);
    }

    [Test]
    public void WalletSpendsOnlyAvailableCoins()
    {
        GameObject gameObject = new GameObject("WalletTest");
        WalletSystem wallet = gameObject.AddComponent<WalletSystem>();
        wallet.SetValue(10);
        Assert.IsFalse(wallet.Spend(11));
        Assert.AreEqual(10, wallet.Coins);
        Assert.IsTrue(wallet.Spend(4));
        Assert.AreEqual(6, wallet.Coins);
        Object.DestroyImmediate(gameObject);
    }
}
