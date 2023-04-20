using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrategicFMS
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

            tree.Insert(7);
            tree.Insert(15);
            tree.Insert(23);
            tree.Insert(31);
            tree.Insert(47);

            Console.WriteLine("Inorder Traversal of " +
                            "Constructed Tree");
            tree.InorderTraversal();
        }

        public void InorderTraversal()
        {
            InorderTraversal(root);
        }
        public void Insert(int value)
        {
            Node newNode = new Node(value);
            Node parent = null;
            Node current = root;
        
            // Traverse the tree to find the appropriate position for the new node
            while (current != null)
            {
                parent = current;
                if (value < current.data)
                {
                    current = current.left;
                }
                else
                {
                    current = current.right;
                }
            }
        
            // Set the parent of the new node
            newNode.parent = parent;
        
            // Insert the new node as a child of the parent
            if (parent == null)
            {
                root = newNode;
            }
            else if (value < parent.data)
            {
                parent.left = newNode;
            }
            else
            {
                parent.right = newNode;
            }
        
            // Fix any violations of the Red Black Tree properties
            fixViolation(newNode);
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
