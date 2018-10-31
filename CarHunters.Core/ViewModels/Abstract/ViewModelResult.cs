using System;

namespace CarHunters.Core.ViewModels.Abstract
{
	public class ViewModelResult
    {
		public string parameter { get; private set; }

		public ViewModelResult()
		{
		}

		public ViewModelResult(object parameter)
		{
			this.parameter = Newtonsoft.Json.JsonConvert.SerializeObject(parameter);
		}

		public T Deserialize<T>()
		{
			if (parameter == null)
				throw new Exception("Missing ViewModelResult value");

			return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(parameter);
		}
	}
}
