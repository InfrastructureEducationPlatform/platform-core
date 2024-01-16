namespace BlockInfrastructure.Core.Configurations;

public class ConnectionConfiguration
{
    /// <summary>
    ///     Database 연결 문자열
    /// </summary>
    public string DatabaseConnection { get; set; }

    /// <summary>
    ///     플러그인 주소
    /// </summary>
    public string DeploymentPluginConnection { get; set; }
}