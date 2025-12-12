using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Rise.Client.Components;
using Xunit;

namespace Rise.Client.Tests.Components;

public class ToggleShould : TestContext
{
    public ToggleShould()
    {
        Services.AddLocalization();
    }

    #region Rendering

    [Fact]
    public void RenderCorrectly_WithDefaultValues()
    {
        // Act
        var cut = RenderComponent<Toggle>();

        // Assert
        var input = cut.Find("input[type='checkbox']");
        Assert.NotNull(input);
        Assert.False(input.HasAttribute("checked"));
        Assert.False(input.HasAttribute("disabled"));
    }

    [Fact]
    public void RenderAsChecked_WhenValueIsTrue()
    {
        // Act
        var cut = RenderComponent<Toggle>(parameters => parameters
            .Add(p => p.Value, true));

        // Assert
        var input = cut.Find("input[type='checkbox']");
        Assert.True(input.HasAttribute("checked"));
    }

    [Fact]
    public void RenderAsUnchecked_WhenValueIsFalse()
    {
        // Act
        var cut = RenderComponent<Toggle>(parameters => parameters
            .Add(p => p.Value, false));

        // Assert
        var input = cut.Find("input[type='checkbox']");
        Assert.False(input.HasAttribute("checked"));
    }

    [Fact]
    public void RenderAsDisabled_WhenDisabledIsTrue()
    {
        // Act
        var cut = RenderComponent<Toggle>(parameters => parameters
            .Add(p => p.Disabled, true));

        // Assert
        var input = cut.Find("input[type='checkbox']");
        Assert.True(input.HasAttribute("disabled"));
    }

    [Fact]
    public void RenderWithCustomId_WhenIdIsProvided()
    {
        // Act
        var cut = RenderComponent<Toggle>(parameters => parameters
            .Add(p => p.Id, "my-custom-id"));

        // Assert
        var input = cut.Find("input[type='checkbox']");
        var label = cut.Find("label");
        Assert.Equal("my-custom-id", input.GetAttribute("id"));
        Assert.Equal("my-custom-id", label.GetAttribute("for"));
    }

    [Fact]
    public void GenerateUniqueId_WhenIdIsNotProvided()
    {
        // Act
        var cut1 = RenderComponent<Toggle>();
        var cut2 = RenderComponent<Toggle>();

        // Assert
        var input1 = cut1.Find("input[type='checkbox']");
        var input2 = cut2.Find("input[type='checkbox']");

        Assert.NotNull(input1.GetAttribute("id"));
        Assert.NotNull(input2.GetAttribute("id"));
        Assert.NotEqual(input1.GetAttribute("id"), input2.GetAttribute("id"));
    }

    #endregion

    #region Value Changes

    [Fact]
    public void InvokeValueChanged_WhenToggled()
    {
        // Arrange
        var valueChangedValue = false;
        var cut = RenderComponent<Toggle>(parameters => parameters
            .Add(p => p.Value, false)
            .Add(p => p.ValueChanged, EventCallback.Factory.Create<bool>(this, (bool value) => valueChangedValue = value)));

        // Act
        cut.Find("input[type='checkbox']").Change(new ChangeEventArgs { Value = true });

        // Assert
        Assert.True(valueChangedValue);
    }

    [Fact]
    public void InvokeOnChange_WhenToggled()
    {
        // Arrange
        var onChangeInvoked = false;
        var cut = RenderComponent<Toggle>(parameters => parameters
            .Add(p => p.Value, false)
            .Add(p => p.OnChange, EventCallback.Factory.Create<bool>(this, (bool _) => onChangeInvoked = true)));

        // Act
        cut.Find("input[type='checkbox']").Change(new ChangeEventArgs { Value = true });

        // Assert
        Assert.True(onChangeInvoked);
    }

    [Fact]
    public void InvokeBothCallbacks_WhenToggled()
    {
        // Arrange
        var valueChangedValue = false;
        var onChangeInvoked = false;

        var cut = RenderComponent<Toggle>(parameters => parameters
            .Add(p => p.Value, false)
            .Add(p => p.ValueChanged, EventCallback.Factory.Create<bool>(this, (bool value) => valueChangedValue = value))
            .Add(p => p.OnChange, EventCallback.Factory.Create<bool>(this, (bool _) => onChangeInvoked = true)));

        // Act
        cut.Find("input[type='checkbox']").Change(new ChangeEventArgs { Value = true });

        // Assert
        Assert.True(valueChangedValue);
        Assert.True(onChangeInvoked);
    }

    [Fact]
    public void ToggleFromTrueToFalse()
    {
        // Arrange
        var valueChangedValue = true;
        var cut = RenderComponent<Toggle>(parameters => parameters
            .Add(p => p.Value, true)
            .Add(p => p.ValueChanged, EventCallback.Factory.Create<bool>(this, (bool value) => valueChangedValue = value)));

        // Act
        cut.Find("input[type='checkbox']").Change(new ChangeEventArgs { Value = false });

        // Assert
        Assert.False(valueChangedValue);
    }

    [Fact]
    public async Task UpdateValue_WhenTwoWayBindingIsUsed()
    {
        // Arrange
        var value = false;
        var cut = RenderComponent<Toggle>(parameters => parameters
            .Add(p => p.Value, value)
            .Add(p => p.ValueChanged, EventCallback.Factory.Create<bool>(this, (bool v) => value = v)));

        // Act
        cut.Find("input[type='checkbox']").Change(new ChangeEventArgs { Value = true });

        // Assert
        Assert.True(value);
    }

    #endregion

    #region Disabled State

    [Fact]
    public void NotBeInteractive_WhenDisabled()
    {
        // Arrange
        var valueChangedValue = false;
        var cut = RenderComponent<Toggle>(parameters => parameters
            .Add(p => p.Value, false)
            .Add(p => p.Disabled, true)
            .Add(p => p.ValueChanged, EventCallback.Factory.Create<bool>(this, (bool value) => valueChangedValue = value)));

        // Assert - disabled input should not be clickable
        var input = cut.Find("input[type='checkbox']");
        Assert.True(input.HasAttribute("disabled"));
    }

    #endregion

    #region Styling

    [Fact]
    public void HaveToggleSwitchClass()
    {
        // Act
        var cut = RenderComponent<Toggle>();

        // Assert
        var label = cut.Find("label");
        Assert.Contains("toggle-switch", label.ClassList);
    }

    [Fact]
    public void HaveDisabledClass_WhenDisabled()
    {
        // Act
        var cut = RenderComponent<Toggle>(parameters => parameters
            .Add(p => p.Disabled, true));

        // Assert
        var label = cut.Find("label");
        Assert.Contains("disabled", label.ClassList);
    }

    [Fact]
    public void HaveSliderElement()
    {
        // Act
        var cut = RenderComponent<Toggle>();

        // Assert
        var slider = cut.Find("span.toggle-slider");
        Assert.NotNull(slider);
    }

    #endregion
}
