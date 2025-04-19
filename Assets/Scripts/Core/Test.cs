using System;
using UnityEngine;

namespace Core
{
    public delegate bool TW(int a, string b, Vector3 pos);
    
    public class Test
    {
        public Action action1;
        public TW action2;
        
        
        public Vector3 position;

        public void Init()
        {
            Foo
            (
                (int a, string b, Vector3 pos) =>
            {
                return true;
            }
                );
            
            action2(1, "", Vector3.zero);

            action2 = Stop;
            action2(1, "", Vector3.zero);
        }

        public void Foo(TW tw)
        {
            tw(1, "", Vector3.zero);
        }
        
        public bool Run(int a, string b, Vector3 pos)
        {
            return true;
        }
        
        public bool Stop(int a, string b, Vector3 pos)
        {
            return false;
        }
    }
}