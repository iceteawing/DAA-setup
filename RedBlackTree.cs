using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAA_setup
{
    enum Color { RED, BLACK };
    class Node
    {
        public int data;
        public Color color;
        public Node left, right, parent;

        public Node(int data)
        {
            this.data = data;
            color = Color.RED;
            left = right = parent = null;
        }
    }
    internal class RedBlackTree
    {
        private Node root;

        public RedBlackTree()
        {
            root = null;
        }

        private void rotateLeft(Node x)
        {
            Node y = x.right;
            x.right = y.left;
            if (y.left != null)
                y.left.parent = x;
            if (y != null)
                y.parent = x.parent;
            if (x.parent == null)
                root = y;
            else if (x == x.parent.left)
                x.parent.left = y;
            else
                x.parent.right = y;
            y.left = x;
            if (x != null)
                x.parent = y;
        }

        private void rotateRight(Node x)
        {
            Node y = x.left;
            x.left = y.right;
            if (y.right != null)
                y.right.parent = x;
            if (y != null)
                y.parent = x.parent;
            if (x.parent == null)
                root = y;
            else if (x == x.parent.right)
                x.parent.right = y;
            else
                x.parent.left = y;
            y.right = x;
            if (x != null)
                x.parent = y;
        }

        private void fixViolation(Node x)
        {
            Node parent = null;
            Node grandparent = null;
            while (x != root && x.color == Color.RED && x.parent.color == Color.RED)
            {
                parent = x.parent;
                grandparent = x.parent.parent;
                if (parent == grandparent.left)
                {
                    Node uncle = grandparent.right;
                    if (uncle != null && uncle.color == Color.RED)
                    {
                        grandparent.color = Color.RED;
                        parent.color = Color.BLACK;
                        uncle.color = Color.BLACK;
                        x = grandparent;
                    }
                    else
                    {
                        if (x == parent.right)
                        {
                            rotateLeft(parent);
                            x = parent;
                            parent = x.parent;
                        }
                        rotateRight(grandparent);
                        Color temp = parent.color;
                        parent.color = grandparent.color;
                        grandparent.color = Color.BLACK;
                        grandparent.color = temp;
                        x = parent;
                    }
                }
                else
                {
                    Node uncle = grandparent.left;
                    if (uncle != null && uncle.color == Color.RED)
                    {
                        grandparent.color = Color.RED;
                        parent.color = Color.BLACK;
                        uncle.color = Color.BLACK;
                        x = grandparent;
                    }
                    else
                    {
                        if (x == parent.left)
                        {
                            rotateRight(parent);
                            x = parent;
                            parent = x.parent;
                        }
                        rotateLeft(grandparent);
                        Color temp = parent.color;
                        parent.color = grandparent.color;
                        grandparent.color = temp;
                        x = parent;
                    }
                }
            }
            root.color = Color.BLACK;
        }
        // Driver program to test above functions
        // Example usage
        public void usage()
        {
            RedBlackTree tree = new RedBlackTree();

            tree.root = new Node(10);
            tree.root.left = new Node(20);
            tree.root.right = new Node(30);
            tree.root.right.right = new Node(40);
            tree.root.right.left = new Node(50);

            Console.WriteLine("Inorder Traversal of " +
                            "Constructed Tree");
            tree.InorderTraversal();
        }

        public void InorderTraversal()
        {
            InorderTraversal(root);
        }

        private void InorderTraversal(Node node)
        {
            if (node != null)
            {
                InorderTraversal(node.left);
                Console.Write(node.data + " ");
                InorderTraversal(node.right);
            }
        }
    }



    
}
