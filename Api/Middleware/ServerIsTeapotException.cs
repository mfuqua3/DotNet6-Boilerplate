namespace Api.Middleware;

public class ServerIsTeapotException : Exception
{
    public ServerIsTeapotException():base("I'm a teapot")
    {   
    }
}