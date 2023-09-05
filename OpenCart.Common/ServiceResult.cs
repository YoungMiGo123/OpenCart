namespace OpenCart.Common
{
    public class ServiceResult<T>
    {
        public ServiceResult()
        {
            Errors = new List<string>();
        }
        public ServiceResult(T response) : this()
        {
            Response = response;
        }
        public ServiceResult(string error) 
        {
            Errors = new List<string>() { error };
        }
        public T Response { get; set; }

        public dynamic Tag { get; set; }

        public bool HasErrors => Errors.Any();

        public List<string> Errors { get; set; }
    }
}
