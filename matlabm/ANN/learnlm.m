function j = learnlm(p,d)
%LEARNLM Levenberg-Marquardt learning rule.
%	
%	LEARNLM(P,D)
%	  P  - RxQ matrix of input (column) vectors.
%	  D  - SxQ matrix of delta (column) vectors.
%	Returns:
%	  Partial jacobian matrix.
%	
%	See also NNLEARN, BACKPROP, INITFF, SIMFF, TRAINLM.

% Mark Beale, 12-15-93
% Copyright (c) 1992-94 by the MathWorks, Inc.
% $Revision: 1.1 $  $Date: 1994/01/11 16:25:19 $

if nargin < 2, error('Wrong number of arguments.'),end

[R,Q]=size(p);
[S,Q]=size(d);
j = nncpy(d',R) .* nncpyi(p',S);
