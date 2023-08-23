namespace MatrixWeb.Shared.Inputs;

public class HourInput : ExtendedInput{
    protected override void OnParametersSet() {
        this.Text = "Hours";
        base.OnParametersSet();
    }
}
