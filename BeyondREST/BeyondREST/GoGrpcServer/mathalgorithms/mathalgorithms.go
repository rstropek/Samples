package mathalgorithms

// FibonacciIterator generates fibonacci numbers
type FibonacciIterator struct {
	previous int32
	current  int32
}

// NewFibonacciIterator creates a new iterator generating fibonacci numbers
func NewFibonacciIterator() FibonacciIterator {
	return FibonacciIterator{
		previous: 0,
		current:  0,
	}
}

// Next moves iterator to next fibonacci number
func (fi *FibonacciIterator) Next() bool {
	if fi.current == 0 && fi.previous == 0 {
		fi.current = 1
		return true
	}

	fi.previous, fi.current = fi.current, fi.previous+fi.current
	return true
}

// Value returns current fibonacci number
func (fi FibonacciIterator) Value() int32 {
	if fi.current == 0 && fi.previous == 0 {
		panic("Iterator in invalid state, call Next at least once before calling Value")
	}

	return fi.current
}
