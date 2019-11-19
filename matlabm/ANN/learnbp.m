function [dw,db] = learnbp(p,d,lr)
%LEARNBP Backpropagation learning rule.
%	
%	[dW,dB] = LEARNBP(P,D,LR)
%	  P  - RxQ matrix of input vectors.
%	  E  - SxQ matrix of error vectors.
%	  LR - the learning rate (default = 1).
%	Returns:
%	  dW - a weight change matrix.
%	  dB - a bias change vector (optional).
%	
%	See also NNLEARN, BACKPROP, SIMFF, INITFF, TRAINBP.

% Mark Beale, 1-31-92
% Revised 12-15-93, MB
% Copyright (c) 1992-93 by the MathWorks, Inc.
% $Revision: 1.1 $  $Date: 1994/01/11 16:25:01 $

if nargin < 2, error('Not enough arguments.'); end
if nargin == 3, d = lr*d; end

dw = d*p';
if nargout == 2
  [R,Q] = size(p);
  db = d*ones(Q,1);
end
