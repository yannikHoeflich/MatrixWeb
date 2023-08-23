namespace MatrixWeb.Shared.Inputs;

public class PercentInput : ExtendedInput {
    protected override void OnParametersSet() {
        Text = "%";
        Multiplier = 100;
        base.OnParametersSet();
    }
}
