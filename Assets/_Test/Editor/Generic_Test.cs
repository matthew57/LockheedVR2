using UnityEngine;
using UnityEditor;
using NUnit.Framework;

using IVR;

public class AngleUtils_Test {

    [Test]
    public void Clamp_TestSmaller() {
        // Arrange
        float a = 10;

        // Act
        float r = Angles.Clamp(a, 20, 40);

        // Asset
        Assert.GreaterOrEqual(r, 20);
        Assert.LessOrEqual(r, 40);
    }

    [Test]
    public void Clamp_TestMin() {
        // Arrange
        float a = 20;

        // Act
        float r = Angles.Clamp(a, 20, 40);

        // Asset
        Assert.GreaterOrEqual(r, 20);
        Assert.LessOrEqual(r, 40);
    }

    [Test]
    public void Clamp_TestInRange() {
        // Arrange
        float a = 30;

        // Act
        float r = Angles.Clamp(a, 20, 40);

        // Asset
        Assert.GreaterOrEqual(r, 20);
        Assert.LessOrEqual(r, 40);
    }

    [Test]
    public void Clamp_TestMax() {
        // Arrange
        float a = 40;

        // Act
        float r = Angles.Clamp(a, 20, 40);

        // Asset
        Assert.GreaterOrEqual(r, 20);
        Assert.LessOrEqual(r, 40);
    }

    [Test]
    public void Clamp_TestGreater() {
        // Arrange
        float a = 50;

        // Act
        float r = Angles.Clamp(a, 20, 40);

        // Asset
        Assert.GreaterOrEqual(r, 20);
        Assert.LessOrEqual(r, 40);
    }

    [Test]
    public void Clamp_TestInverse() {
        // Arrange
        float a = 30;

        // Act
        float r = Angles.Clamp(a, 40, 20);

        // Asset
        Assert.GreaterOrEqual(r, 20);
        Assert.LessOrEqual(r, 40);
    }

    [Test]
    public void Clamp_TestInverseSmaller() {
        // Arrange
        float a = 10;

        // Act
        float r = Angles.Clamp(a, 40, 20);

        // Asset
        Assert.GreaterOrEqual(r, 20);
        Assert.LessOrEqual(r, 40);
    }

    [Test]
    public void Clamp_TestNoRange() {
        // Arrange
        float a = 30;

        // Act
        float r = Angles.Clamp(a, 30, 30);

        // Asset
        Assert.GreaterOrEqual(r, 30);
        Assert.LessOrEqual(r, 30);
    }
    [Test]
    public void Difference_TestPos() {
        // Arrange
        float a = 10;
        float b = 20;

        // Act
        float r = Angles.Difference(a, b);

        // Asset
        Assert.AreEqual(r, 10);
    }

    [Test]
    public void Difference_TestNeg() {
        // Arrange
        float a = 20;
        float b = 10;

        // Act
        float r = Angles.Difference(a, b);

        // Asset
        Assert.AreEqual(r, -10);
    }

    [Test]
    public void Difference_TestEqual() {
        // Arrange
        float a = 10;
        float b = 10;

        // Act
        float r = Angles.Difference(a, b);

        // Asset
        Assert.AreEqual(r, 0);
    }

    [Test]
    public void NormalizeAngle_TestMin() {
        //Arrange
        float a = -180;

        //Act
        float r = Angles.Normalize(a);

        //Assert
        Assert.Greater(r, -180);
        Assert.LessOrEqual(r, 180);
    }

    [Test]
    public void NormalizeAngle_TestMax() {
        //Arrange
        float a = 180;

        //Act
        float r = Angles.Normalize(a);

        //Assert
        Assert.Greater(r, -180);
        Assert.LessOrEqual(r, 180);
    }
}
