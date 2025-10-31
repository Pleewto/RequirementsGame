/// <summary>
/// View for editing an existing scenario.
/// Inherits from ViewCreateScenario, reusing its layout and control structure.
/// All text fields and controls are populated with the provided scenario’s data
/// instead of being left empty as in the base create view.
/// </summary>
public class ViewEditScenario : ViewCreateScenario
{

    public void ChangeScenario(ref Scenario scenario)
    {

        this.referenceScenario = scenario;
        this.editingScenario = new Scenario(scenario);
        this.RebuildView();

    }

}