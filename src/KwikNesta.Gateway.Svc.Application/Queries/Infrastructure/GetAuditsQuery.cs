using KwikNesta.Contracts.Enums;
using KwikNesta.Contracts.Models;

namespace KwikNesta.Gateway.Svc.Application.Queries.Infrastructure
{
    /// <summary>
    /// The audit log query parameter
    /// </summary>
    public class GetAuditsQuery : PageQuery
    {
        /// <summary>
        /// Search by performer name
        /// </summary>
        public string? Search { get; set; }
        /// <summary>
        /// The domain of the event.  
        /// <br/><br/>
        /// <b>Accepted values:</b>
        /// <list type="bullet">
        /// <item><description><c>1</c> = Identity, </description></item>
        /// <item><description><c>2</c> = System, </description></item>
        /// <item><description><c>3</c> = Property, </description></item>
        /// <item><description><c>4</c> = Booking, </description></item>
        /// <item><description><c>5</c> = Payment, </description></item>
        /// <item><description><c>6</c> = Lease, </description></item>
        /// <item><description><c>7</c> = Communication, </description></item>
        /// <item><description><c>8</c> = Security</description></item>
        /// </list>
        /// <br/><br/>
        /// </summary>
        public AuditDomain? Domain { get; set; }

        /// <summary>
        /// The action type performed.  
        /// <br/><br/>
        /// <b>Accepted values:</b>
        /// <list type="bullet">
        /// <item><description><c>1</c> = Logged In, </description></item>
        /// <item><description><c>2</c> = Logged Out, </description></item>
        /// <item><description><c>3</c> = Updated Profile, </description></item>
        /// <item><description><c>4</c> = Deactivated Account, </description></item> 
        /// <item><description><c>5</c> = Reactivated Account, </description></item> 
        /// <item><description><c>6</c> = Suspended Account, </description></item>
        /// <item><description><c>7</c> = Changed Password, </description></item>
        /// <item><description><c>8</c> = DataLoad Request, </description></item>
        /// <item><description><c>9</c> = DataLoad Completed, </description></item> 
        /// <item><description><c>10</c> = Configuration Updated, </description></item>
        /// <item><description><c>11</c> = System Job Executed, </description></item> 
        /// <item><description><c>12</c> = Property Created, </description></item> 
        /// <item><description><c>13</c> = Property Updated, </description></item>
        /// <item><description><c>14</c> = Property Deactivated, </description></item>
        /// <item><description><c>15</c> = Property Reactivated, </description></item>
        /// <item><description><c>16</c> = Property Removed, </description></item> 
        /// <item><description><c>17</c> = Booking Created, </description></item> 
        /// <item><description><c>18</c> = Booking Updated, </description></item> 
        /// <item><description><c>19</c> = Booking Cancelled, </description></item> 
        /// <item><description><c>20</c> = Application Submitted, </description></item> 
        /// <item><description><c>21</c> = Application Approved, </description></item> 
        /// <item><description><c>22</c> = Application Rejected, </description></item> 
        /// <item><description><c>23</c> = Payment Initiated, </description></item> 
        /// <item><description><c>24</c> = Payment Successful, </description></item> 
        /// <item><description><c>25</c> = Payment Failed, </description></item> 
        /// <item><description><c>26</c> = Refund Issued, </description></item> 
        /// <item><description><c>27</c> = Deposit Collected, </description></item> 
        /// <item><description><c>28</c> = Deposit Refunded, </description></item>
        /// <item><description><c>29</c> = Lease Generated, </description></item> 
        /// <item><description><c>30</c> = Lease Signed, </description></item> 
        /// <item><description><c>31</c> = Lease Renewed, </description></item> 
        /// <item><description><c>32</c> = Lease Terminated, </description></item> 
        /// <item><description><c>33</c> = Notification Sent, </description></item> 
        /// <item><description><c>34</c> = Message Sent, </description></item> 
        /// <item><description><c>35</c> = Message Received, </description></item> 
        /// <item><description><c>36</c> = Failed Login Attempt, </description></item> 
        /// <item><description><c>37</c> = Suspicious Activity Detected, </description></item>
        /// <item><description><c>38</c> = Data Exported, </description></item>
        /// <item><description><c>39</c> = Data Accessed</description></item>
        /// </list>
        /// <br/><br/>
        /// </summary>
        public AuditAction? Action { get; set; }
        /// <summary>
        /// The start date of the date range
        /// </summary>
        public DateTime? StartDate { get; set; }
        /// <summary>
        /// The end date of the date range
        /// </summary>
        public DateTime? EndDate { get; set; }
    }
}
