# Gebruik

### Basis gebruik met two-way binding

```razor
<Toggle @bind-Value="isEnabled" />
```

### Met OnChange callback

```razor
<Toggle @bind-Value="notificationsEnabled"
        OnChange="HandleNotificationChange" />

@code {
    private bool notificationsEnabled = true;

    private async Task HandleNotificationChange(bool newValue)
    {
        // Doe iets wanneer de toggle verandert
        Console.WriteLine($"Notifications: {newValue}");
    }
}
```

### Disabled state

```razor
<Toggle Value="true" Disabled="true" />
```

### Met custom ID

```razor
<Toggle @bind-Value="darkMode" Id="dark-mode-toggle" />
```

### Met label

```razor
<div class="flex items-center gap-3">
    <span>Push notificaties</span>
    <Toggle @bind-Value="pushEnabled" />
</div>
```

## Parameters

| Parameter      | Type                  | Default        | Beschrijving                                              |
| -------------- | --------------------- | -------------- | --------------------------------------------------------- |
| `Value`        | `bool`                | `false`        | De huidige waarde van de toggle (true = aan, false = uit) |
| `ValueChanged` | `EventCallback<bool>` | -              | Callback voor two-way binding                             |
| `OnChange`     | `EventCallback<bool>` | -              | Optionele callback wanneer de waarde verandert            |
| `Disabled`     | `bool`                | `false`        | Of de toggle uitgeschakeld is                             |
| `Id`           | `string?`             | auto-generated | Unieke identifier voor de toggle                          |
