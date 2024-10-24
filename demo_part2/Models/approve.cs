namespace demo_part2.Models
{
    public class approve
    {
        public List<approve> approvedList { get; set; } = new List<approve>();

        public int UserId { get; set; }
        public string Email { get; set; }
        public string Module { get; set; }
        public double HoursWorked { get; set; } // Change back to double
        public double HourRate { get; set; } // Change back to double
        public double TotalAmount { get; set; } // Change back to double
        public string Status { get; set; }
        public string Note { get; set; }
        public string DocumentFilename { get; set; }
        public string FeedbackMessage { get; set; }

        public DateTime SubmissionDate { get; set; }
        public DateTime? ApprovalDate { get; set; }
        public DateTime? RejectionDate { get; set; }

        public int? ApprovedByUserId { get; set; }
        public int? RejectedByUserId { get; set; }

        public string RejectionReason { get; set; }

        public bool IsValid { get; set; }

        // Optional: List of comments related to the approval/rejection process
        public List<string> Comments { get; set; } = new List<string>();

        // Method to approve the claim
        public void Approve(int approverId)
        {
            Status = "Approved";
            ApprovalDate = DateTime.Now;
            ApprovedByUserId = approverId;
            IsValid = true;
            FeedbackMessage = "Claim approved successfully.";
            Comments.Add($"Claim approved by User ID: {approverId} on {ApprovalDate.Value.ToShortDateString()}");
        }

        // Method to reject the claim
        public void Reject(int rejectorId, string reason)
        {
            Status = "Rejected";
            RejectionDate = DateTime.Now;
            RejectedByUserId = rejectorId;
            RejectionReason = reason;
            IsValid = false;
            FeedbackMessage = "Claim rejected.";
            Comments.Add($"Claim rejected by User ID: {rejectorId} on {RejectionDate.Value.ToShortDateString()}. Reason: {reason}");
        }
    }
}