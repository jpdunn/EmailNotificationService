namespace EmailNotificationService.Models;

public class EmailContentData
{
    public string FromUsername { get; set; }
    
    public string FromEmail { get; set; }
    
    public string ToCustomerName { get; set; }
    
    public string ToEmailAddress { get; set; }
    
    public string CustomerUsername { get; set; }
}