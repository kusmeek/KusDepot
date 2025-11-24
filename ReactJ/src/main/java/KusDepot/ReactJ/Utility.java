package KusDepot.ReactJ;

import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

public class Utility
{
    private static final Logger logger = LoggerFactory.getLogger(Utility.class);

    public static int getPortFromEnv(String envVar, int defaultPort)
	{
        try
		{
            String portStr = System.getenv(envVar);
            if (portStr != null)
			{
                return Integer.parseInt(portStr);
            }
        }
		catch (Exception e)
		{
            logger.error("Failed to get port from env for {}", envVar, e);
        }
        return defaultPort;
    }
}