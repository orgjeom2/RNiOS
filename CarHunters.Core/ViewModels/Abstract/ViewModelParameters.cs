using System;

namespace CarHunters.Core.ViewModels.Abstract
{
	public class ViewModelParameter
	{
		public string parameter { get; private set; }

		public ViewModelParameter()
		{
		}

		public ViewModelParameter(object parameter)
		{
			this.parameter = Newtonsoft.Json.JsonConvert.SerializeObject(parameter);
		}

		public T Deserialize<T>()
		{
			if (parameter == null)
				throw new Exception("Missing ViewModelParameter value");

			return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(parameter);
		}
	}
}
