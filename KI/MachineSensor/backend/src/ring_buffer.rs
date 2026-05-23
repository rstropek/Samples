#![allow(unused)]

mod ring_buffer_impl {
    pub struct RingBuffer<T> {
        buffer: Vec<Option<T>>,
        head: usize,
    }

    impl<T> RingBuffer<T> {
        pub fn new(capacity: usize) -> Self {
            let mut buffer = Vec::with_capacity(capacity);
            for _ in 0..capacity {
                buffer.push(None);
            }

            Self {
                buffer,
                head: 0,
            }
        }

        pub fn push(&mut self, value: T) {
            self.buffer[self.head] = Some(value);
            self.head = (self.head + 1) % self.buffer.len();
        }

        pub(self) fn get_tail(&self) -> usize {
            if self.buffer[self.head].is_none() {
                0
            } else {
                self.head
            }
        }

        pub fn len(&self) -> usize {
            if self.buffer[self.head].is_none() {
                self.head
            } else {
                self.buffer.len()
            }
        }

        pub fn into_iter(&self) -> RingBufferIterator<T> {
            RingBufferIterator::new(self)
        }
    }

    pub struct RingBufferIterator<'a, T> {
        buffer: &'a RingBuffer<T>,
        current: usize,
        remaining: usize
    }

    impl<'a, T> RingBufferIterator<'a, T> {
        pub fn new(buffer: &'a RingBuffer<T>) -> Self {
            Self {
                buffer,
                current: if buffer.head == 0 { buffer.buffer.len() } else { buffer.head },
                remaining: buffer.len()
            }
        }
    }

    impl<'a, T> Iterator for RingBufferIterator<'a, T> where T: Clone {
        type Item = T;

        fn next(&mut self) -> Option<Self::Item> {
            if self.remaining == 0 {
                return None;
            }   

            self.current = if self.current == 0 { self.buffer.buffer.len() - 1 } else { self.current - 1 };
            self.remaining -= 1;

            let value = &self.buffer.buffer[self.current];
            Some(value.as_ref().unwrap().clone())
        }
    }
}

pub use ring_buffer_impl::{RingBuffer, RingBufferIterator};

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn test_new_ring_buffer() {
        let buffer: RingBuffer<i32> = RingBuffer::new(3);
        assert_eq!(buffer.len(), 0);
    }

    #[test]
    fn test_push_and_len() {
        let mut buffer = RingBuffer::new(3);
        buffer.push(1);
        assert_eq!(buffer.len(), 1);
        buffer.push(2);
        assert_eq!(buffer.len(), 2);
        buffer.push(3);
        assert_eq!(buffer.len(), 3);
        // Test that pushing to a full buffer wraps around
        buffer.push(4);
        assert_eq!(buffer.len(), 3);
    }

    #[test]
    fn test_iterator_empty() {
        let buffer: RingBuffer<i32> = RingBuffer::new(3);
        let items: Vec<i32> = buffer.into_iter().collect();
        assert_eq!(items.len(), 0);
    }

    #[test]
    fn test_iterator_partial() {
        let mut buffer = RingBuffer::new(3);
        buffer.push(1);
        buffer.push(2);
        
        let items: Vec<i32> = buffer.into_iter().collect();
        assert_eq!(items, vec![2, 1]);
    }

    #[test]
    fn test_iterator_full() {
        let mut buffer = RingBuffer::new(3);
        buffer.push(1);
        buffer.push(2);
        buffer.push(3);
        
        let items: Vec<i32> = buffer.into_iter().collect();
        assert_eq!(items, vec![3, 2, 1]);
    }

    #[test]
    fn test_iterator_wrap_around() {
        let mut buffer = RingBuffer::new(3);
        buffer.push(1);
        buffer.push(2);
        buffer.push(3);
        buffer.push(4); // This should overwrite 1
        
        let items: Vec<i32> = buffer.into_iter().collect();
        assert_eq!(items, vec![4, 3, 2]);
    }

    #[test]
    fn test_multiple_wraps() {
        let mut buffer = RingBuffer::new(2);
        buffer.push(1);
        buffer.push(2);
        buffer.push(3);
        buffer.push(4);
        buffer.push(5);
        
        let items: Vec<i32> = buffer.into_iter().collect();
        assert_eq!(items, vec![5, 4]);
    }
}
