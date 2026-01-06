namespace ApartmentManager.Maui;

public class ValidationBehaviour : Behavior<Editor>
{
    protected override void OnAttachedTo(Editor entry)
    {
        entry.TextChanged += OnEntryTextChanged;
        base.OnAttachedTo(entry);
    }

    protected override void OnDetachingFrom(Editor entry)
    {
        entry.TextChanged -= OnEntryTextChanged;
        base.OnDetachingFrom(entry);
    }

    private void OnEntryTextChanged(object sender, TextChangedEventArgs args)
    {
        ((Editor)sender).BackgroundColor =
            string.IsNullOrEmpty(args.NewTextValue) ? Color.FromRgba("#AA4A44") : Color.FromRgba("#FFFFFF");
    }
}