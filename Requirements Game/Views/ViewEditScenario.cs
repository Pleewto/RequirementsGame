public class ViewEditScenario : ViewCreateScenario {

    public void ChangeScenario(ref Scenario scenario) {

        this.referenceScenario = scenario;
        this.editingScenario = new Scenario(scenario);
        //this.isEditMode = true;
        this.RebuildView();
    }
}