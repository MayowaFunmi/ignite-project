namespace ignite_project.Constants
{
  public enum WithdrawalStatus
  {
    UNREQUESTED,
    PENDING,
    INCOMPLETE, // withdrawal successfull but not yet effected in the database
    REJECTED,
    APPROVED,
    CANCELED
  }
}