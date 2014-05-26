import org.apache.http.HttpHost;
import org.apache.http.HttpResponse;
import org.apache.http.NameValuePair;
import org.apache.http.auth.AuthScope;
import org.apache.http.auth.UsernamePasswordCredentials;
import org.apache.http.client.AuthCache;
import org.apache.http.client.entity.UrlEncodedFormEntity;
import org.apache.http.client.methods.HttpPost;
import org.apache.http.client.protocol.ClientContext;
import org.apache.http.impl.auth.BasicScheme;
import org.apache.http.impl.client.AbstractHttpClient;
import org.apache.http.impl.client.BasicAuthCache;
import org.apache.http.impl.client.DefaultHttpClient;
import org.apache.http.message.BasicNameValuePair;
import org.apache.http.protocol.BasicHttpContext;

import com.google.gson.JsonObject;
import java.net.InetAddress;
import java.net.NetworkInterface;

import com.sun.management.OperatingSystemMXBean;
import java.lang.management.ManagementFactory;
import java.text.SimpleDateFormat;
//import java.lang.management.OperatingSystemMXBean;
import java.util.ArrayList;
import java.util.Calendar;
import java.util.Date;
import java.util.List;

import javax.management.MBeanServerConnection;



public class Sensor {

	/**
	 * @param args
	 */
	public static void main(String[] args) throws Exception{
		
		System.out.println("hello");
		
		HttpHost targetHost = new HttpHost("localhost", 8080, "http");
		AbstractHttpClient httpclient = new DefaultHttpClient();
        httpclient.getCredentialsProvider().setCredentials(
                new AuthScope(targetHost.getHostName(), targetHost.getPort()),
                new UsernamePasswordCredentials("test@liferay.com", "test"));
 
        // Create AuthCache instance
        AuthCache authCache = new BasicAuthCache();
        BasicScheme basicAuth = new BasicScheme();
        authCache.put(targetHost, basicAuth);

        BasicHttpContext ctx = new BasicHttpContext();
        ctx.setAttribute(ClientContext.AUTH_CACHE, authCache);

        HttpPost post = new HttpPost("/api/secure/json?");
        
        SimpleDateFormat df = new SimpleDateFormat("dd-MM-yy:HH:mm:ss");
        String hostname = InetAddress.getLocalHost().getHostName();
        InetAddress ip;
        ip = InetAddress.getLocalHost();
        NetworkInterface network = NetworkInterface.getByInetAddress(ip);
        byte[] mac = network.getHardwareAddress();
        
        StringBuilder sb = new StringBuilder();
		for (int i = 0; i < mac.length; i++) {
			sb.append(String.format("%02X", mac[i]));	
		}
		
		com.sun.management.OperatingSystemMXBean bean = (com.sun.management.OperatingSystemMXBean) java.lang.management.ManagementFactory.getOperatingSystemMXBean();
	
	//	while(true)
		for(int i=0; i<5; i++)
		{
			JsonObject ob = new JsonObject();
			ob.addProperty("resource_id", sb.toString());
			ob.addProperty("resource_name", hostname);
			ob.addProperty("measurement_time", df.format(new Date()));
			ob.addProperty("resource_cpu_cores", bean.getAvailableProcessors());
			ob.addProperty("resource_total_memory", bean.getTotalPhysicalMemorySize()/1000000);
			ob.addProperty("resource_free_memory", bean.getFreePhysicalMemorySize()/1000000);
			ob.addProperty("resource_cpu_usage", bean.getSystemCpuLoad()*100);

	        List<NameValuePair> params = new ArrayList<NameValuePair>();
	        params.add(new BasicNameValuePair("serviceClassName", "monitor.service.MeasurementServiceUtil"));
	        params.add(new BasicNameValuePair("serviceMethodName", "setMeasurement"));
	        params.add(new BasicNameValuePair("serviceParameters", "[measurementId]"));
	        params.add(new BasicNameValuePair("measurementId", ob.toString()));
	        UrlEncodedFormEntity entity = new UrlEncodedFormEntity(params, "UTF-8");
	        post.setEntity(entity);
	
	        HttpResponse resp = httpclient.execute(targetHost, post, ctx);
	        System.out.println(resp.getStatusLine());
	        resp.getEntity().writeTo(System.out);
	        System.out.println();
	        
	        Thread.sleep(2000);
		}
		
        httpclient.getConnectionManager().shutdown();
		
    }


}
