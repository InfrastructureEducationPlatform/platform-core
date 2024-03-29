namespace BlockInfrastructure.Common.Models.Errors;

public class DeploymentError : IErrorTitle
{
    private readonly ErrorTitle _errorTitle;

    public static DeploymentError DeploymentNotFound => new(ErrorTitle.DeploymentNotFound);
    public static DeploymentError CannotDeleteNonLatestDeployment => new(ErrorTitle.CannotDeleteNonLatestDeployment);

    private DeploymentError(ErrorTitle errorTitle)
    {
        _errorTitle = errorTitle;
    }

    public string ErrorTitleToString()
    {
        return _errorTitle.ToString();
    }

    private enum ErrorTitle
    {
        DeploymentNotFound,
        CannotDeleteNonLatestDeployment
    }
}