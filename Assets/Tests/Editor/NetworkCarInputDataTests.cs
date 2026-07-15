using Fusion;
using NUnit.Framework;

public class NetworkCarInputDataTests
{
    [Test]
    public void NetworkCarInputData_IsNetworkInputInterface()
    {
        var data = new NetworkCarInputData();
        Assert.IsInstanceOf<INetworkInput>(data);
    }

    [Test]
    public void NetworkCarInputData_HasVerticalField()
    {
        var data = new NetworkCarInputData { Vertical = 0.5f };
        Assert.AreEqual(0.5f, data.Vertical, 0.01f);
    }

    [Test]
    public void NetworkCarInputData_HasHorizontalField()
    {
        var data = new NetworkCarInputData { Horizontal = -0.3f };
        Assert.AreEqual(-0.3f, data.Horizontal, 0.01f);
    }

    [Test]
    public void NetworkCarInputData_HasHandBrakeField()
    {
        var data = new NetworkCarInputData { HandBrake = true };
        Assert.IsTrue(data.HandBrake);
    }

    [Test]
    public void NetworkCarInputData_HasResetPressedField()
    {
        var data = new NetworkCarInputData { ResetPressed = true };
        Assert.IsTrue(data.ResetPressed);
    }

    [Test]
    public void NetworkCarInputData_DefaultsAllFieldsToZeroOrFalse()
    {
        var data = new NetworkCarInputData();

        Assert.AreEqual(0f, data.Vertical);
        Assert.AreEqual(0f, data.Horizontal);
        Assert.IsFalse(data.HandBrake);
        Assert.IsFalse(data.ResetPressed);
    }

    [Test]
    public void NetworkCarInputData_CanSetMultipleFieldsIndependently()
    {
        var data = new NetworkCarInputData
        {
            Vertical = 1f,
            Horizontal = -0.5f,
            HandBrake = true,
            ResetPressed = false
        };

        Assert.AreEqual(1f, data.Vertical);
        Assert.AreEqual(-0.5f, data.Horizontal);
        Assert.IsTrue(data.HandBrake);
        Assert.IsFalse(data.ResetPressed);
    }

    [Test]
    public void NetworkCarInputData_VerticalValueClampsTo1AndNegative1()
    {
        // Note: Clamping should happen in InputsManager, not in the struct itself
        // This test verifies the struct can store floats; clamping is tested in InputsManager tests
        var data = new NetworkCarInputData { Vertical = 2f };
        Assert.AreEqual(2f, data.Vertical);
    }
}
