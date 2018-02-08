using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary1
{
    /**
     *  This is an implementation of a simple binary tree and used for 
     *  binary operations on a group set 
     *  in order to parse the boollian quary we will construct a 
     *  binary tree on the groups of file ids and create the 
     *  binary formula of the group using the tree
     *  finally we will ask for the head of the tree to calculate the formulation 
     *  using a recursion methode on the tree too go down each level
     */
    public abstract class BinaryTree<T>
    {
        public abstract List<T> GetValue();
    }

    public class GroupNode<T> : BinaryTree<T>
    {
        public GroupNode(List<T> data)
        {
            m_data = data;
        }

        public override List<T> GetValue()
        {
            return m_data;
        }

        private List<T> m_data;
    }

    public class OrTreeNode<T> : BinaryTree<T>
    {
        public OrTreeNode(BinaryTree<T> left, BinaryTree<T> right)
        {
            m_left = left;
            m_right = right;
        }

        public override List<T> GetValue()
        {
            // create a new group containing all the object's from both groups
            return m_left.GetValue().Concat(m_right.GetValue()).Distinct().ToList();
        }
        protected BinaryTree<T> m_left;
        protected BinaryTree<T> m_right;
    }

    public class AndTreeNode<T> : BinaryTree<T>
    {
        public AndTreeNode(BinaryTree<T> left, BinaryTree<T> right) 
        {
            m_left = left;
            m_right = right;
        }

        public override List<T> GetValue()
        {
            // creates a new group with all the object found in both groups
            return m_left.GetValue().Intersect(m_right.GetValue()).Distinct().ToList();
        }

        protected BinaryTree<T> m_left;
        protected BinaryTree<T> m_right;
    }

    public class NotTreeNode<T> : BinaryTree<T>
    {
        // baseNode - the node holding the group to take the set from
        // excludeNode - the node to use as a set of black list operators to remove
        public NotTreeNode(BinaryTree<T> baseNode, BinaryTree<T> excludeNode)
        {
        }

        public override List<T> GetValue()
        {
            // creates a new group with all the object from the base without any object from the black group
            return m_baseNode.GetValue().Except(m_excludeNode.GetValue()).Distinct().ToList();
        }

        private BinaryTree<T> m_baseNode;
        private BinaryTree<T> m_excludeNode;


    }

}
