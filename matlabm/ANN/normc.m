function n = normc(m)
%NORMC Normalize columns of matrix.
%	
%	NORMC(M)
%	  M - a matrix.
%	Returns a matrix the same size with each
%	column normalized to a vector length of 1.
%	
%	See also NORMR, PNORMC.

% Mark Beale, 1-31-92
% Copyright (c) 1992-94 by the MathWorks, Inc.
% $Revision: 1.1 $  $Date: 1994/01/11 16:27:06 $

if nargin < 1,error('Not enough input arguments.'); end

[mr,mc] = size(m);
if (mr == 1)
  n = ones(1,mc);
else
  n =ones(mr,1)*sqrt(ones./sum(m.*m)).*m;
end
