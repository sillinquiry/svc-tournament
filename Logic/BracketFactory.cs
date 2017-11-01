using System;
using System.Collections.Generic;
using System.Linq;

namespace svc_tournament.Logic {
    public static class BracketFactory {
        public class Node<T> {
            public T Content { get; set; }
            public Node<T> Left { get; set; }
            public Node<T> Right { get; set; }
        }


        public static Node<T> Convert<T>(IEnumerable<T> data) {
            var workspace = new Queue<Node<T>>();
            var source = data.Select(d => new Node<T>{Content = d}).ToList();

            var root = source[0];
            workspace.Enqueue(root);
            for(int i = 1; i < source.Count; ++i) {
                var n = workspace.Dequeue();
                n.Left = new Node<T> {
                    Content = n.Content
                };
                n.Right = source[i];
                n.Content = default(T);

                workspace.Enqueue(n.Left);
                workspace.Enqueue(n.Right);
            }

            return root;
        }
    }
}