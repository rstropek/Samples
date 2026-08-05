# Work in the context of one partner

At the moment every screen stands on its own, and an order picks its partner
from a dropdown. Turn that around and make the partner list the entry point:
the user selects a partner there, and from then on the application works in the
context of that partner until they select a different one.

Anything partner-related follows from that selection instead of asking for it
again. Order creation is the obvious case. The partner dropdown disappears from
the form, and the new order belongs to the active partner. The order list
should behave accordingly.

The active partner has to be visible at all times, and switching has to be
quick. People sit in this application all day.

Products are independent of the selection and stay as they are.

Plan this before you write code, and ask me about whatever the description
leaves open.
