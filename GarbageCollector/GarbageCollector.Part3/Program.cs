using System;

namespace GarbageCollector.Part3
{
    /*Microsof уже подготовила формализованный шаблон, и он чертовски запутанный
     поэтому разберу ка я его по кускам...*/
    internal class Car : IDisposable
    {
        private string MethodName { get; }

        private bool _disposed = false;

        public Car(string methodName)
        {
            MethodName = methodName;
        }


        public void Dispose()
        {
            Dispose(true);

            /*Говорим финализатору что этот объект не надо финализировать так как
             метод Dispose(true) освобождает как управляемые так и неуправляемые ресурсы*/
            GC.SuppressFinalize(this);
        }

        /*если _disposed == false это означает что неуправляемые ресурсы не освобождены и запускается дальнейшея логика:
         далее если disoising == false это означает что пользователь по каким либо причинам
         не вызвал метод Dispose(), а данный метод Dispose() был вызван деструкторм, в данной ситуации
         освобождаются только неуправляемые ресурсы. Если же disposing == true - это означает что пользователь
         вызвал Dispose в конструкции using(){} либо явно (car.Dispose()), значит он уверен что управляемые и неуправляемые
         ресурсы ему уже не нужны. Соответственно освобождаются как управляемые так и неуправляемые ресурсы*/
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    /*Здесь освоюождаются управляемые ресурсы*/
                    Console.WriteLine($"Освобождаем управляемые ресурсы в: {MethodName}");
                }

                /*Здесь освобождаются неуправляемые ресурсы*/
                Console.WriteLine($"Освобождаем неуправляемые ресурсы в: {MethodName}");
                Console.Beep();
                /*Тут мы например закрываем подключение к БД*/

                _disposed = true;
            }
        }

        /*Когда вызывается деструктор? Тогда когда пользователь забывает вызвать метод
         Disoise() вручную либо поместить обьект в конструкцию using(){} - тогда сборщик мусора
         вызывает деструктор для финализации. Важно понимать для чего в деструкторе вызывается Dispose(false)
         с параметром false: потому что параметр disposing отвечет за очистку управляемых ресурсов и когда
         пользователь не вызывает Dispose мы не можем быть уверены что управляемые ресурсы до сих пор
         находятся в памяти, остается полагаться на деструкторы управляемых ресурсов. Поэтому при вызове
         метода Dispose(false) освобождаются только неуправляемые ресурсы.*/
        ~Car()
        {
            Dispose(false);
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            WithoutDispose();

            WithUsing();

            JustDispose();

            GC.Collect();

            Console.ReadKey();
        }

        /*В данном случае если разработчик не использует метод Dispose()
         все равно произойдет освобождение неуправляемых ресурсов при первой же
         сборке мусора поэтому в методе Main() я запустил GC.Collect (сборку мусора) 
         принудительно. Обратите внимание что в данной ситуации освободяться только 
         неуправляемые ресурсы*/
        private static void WithoutDispose()
        {
            var car = new Car("WithoutDispose() Method");
        }

        /*В данном случае освобождаются как управляемые так и неуправляемые ресурсы*/
        private static void WithUsing()
        {
            using (var car = new Car("WithUsing() Method"))
            {  
            }
        }

        /*В данном случае освобождаются как управляемые так и неуправляемые ресурсы*/
        private static void JustDispose()
        {
            var car = new Car("JustDispose()");

            car.Dispose();
        }
    }
}
