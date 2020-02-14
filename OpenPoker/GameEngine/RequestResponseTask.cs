using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpenPoker.GameEngine
{
    public class RequestResponseTask<T>
    {
        private Task Request;
        public RequestResponseTask(Task request)
        {
            Request = request;
            tcs = new TaskCompletionSource<T>();
        }
        public async Task<T> Run()
        {
            await Request;
            T res = await tcs.Task;
            tcs = null;
            return res;
        }
        TaskCompletionSource<T> tcs = null;
        public bool IsPending
        {
            get
            {
                return tcs != null;
            }
        }
        public void MessageReceived(T value)
        {
            tcs?.TrySetResult(value);
        }
    }
}
