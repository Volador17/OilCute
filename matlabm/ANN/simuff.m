function [a1,a2,a3] = simuff(p,w1,b1,f1,w2,b2,f2,w3,b3,f3)
%SIMUFF Simulate feed-forward network.
%	
%	SIMUFF will simulate networks with up to 3 layers.
%	
%	SIMUFF(P,W1,B1,'F1',...,Wn,Bn,'Fn')
%	  P  - Matrix of input (column) vectors.
%	  Wi - Weight matrix of the ith layer.
%	  Bi - Bias (column) vector of the ith layer.
%	  Fi - Transfer function of the ith layer (string).
%	Returns output of nth layer.
%	
%	[A1,A2,...] = SIMUFF(P,W1,B1,'F1',...,Wn,Bn,'Fn')
%	Returns:
%	  Ai - Output of the ith layer.
%
%	EXAMPLE: [w1,b1,w2,b2] = initff([0 10; -5 5],3,'tansig',2,'purelin');
%	         p = [2; -3];
%	         a = simuff(p,w1,b1,'tansig',w2,b2,'purelin')
%	
%	See also NNSIM, BACKPROP, INITFF, TRAINBPX, TRAINLM.

% Mark Beale, 12-15-93
% Copyright (c) 1992-94 by the MathWorks, Inc.
% $Revision: 1.1 $  $Date: 1994/01/11 16:28:50 $

if all([4 7 10] ~= nargin),error('Wrong number of input arguments'),end

if nargin == 4
  a1 = feval(f1,w1*p,b1);

elseif nargin == 7
  a1 = feval(f1,w1*p,b1);
  a2 = feval(f2,w2*a1,b2);
  if nargout <= 1, a1 = a2; end

elseif nargin == 10
  a1 = feval(f1,w1*p,b1);
  a2 = feval(f2,w2*a1,b2);
  a3 = feval(f3,w3*a2,b3);
  if nargout <= 1, a1 = a3; end
end
