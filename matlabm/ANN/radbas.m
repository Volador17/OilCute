function a = radbas(n,b)
%RADBAS Radial basis transfer function.
%	
%	RADBAS(N)
%	  N - SxQ matrix of distance vectors.
%	Returns values of N passed through radial basis function.
%	
%	EXAMPLE: n = -4:0.1:4;
%	         a = radbas(n);
%	         plot(n,a)
%	
%	RADBAS(Z,B) ...used when batching.
%	  Z - SxQ matrix of unbiased distance vectors.
%	  B - Sx1 bias vector.
%	Obtains N by multiplying elements in each column of Z by
%	  the elements in B, then returns RADBAS(N).
%	
%	RADBAS('delta') returns name of delta function.
%	RADBAS('init') returns name of initialization function.
%	RADBAS('name') returns full name of this transfer function.
%	RADBAS('output') returns output range of this function.
%	
%	See also NNTRANS, RADBASIS, DIST, SIMRB, SOLVERB, SOLVERBE.

% Mark Beale, 12-15-93
% Copyright (c) 1992-94 by The MathWorks, Inc.
% $Revision: 1.1 $  $Date: 1994/01/11 16:28:18 $

if nargin < 1, error('Not enough arguments.'); end

if isstr(n)
  if strcmp(lower(n),'delta')
    a = 'none';
  elseif strcmp(lower(n),'init')
    a = 'rands';
  elseif strcmp(lower(n),'name')
    a = 'Radial Basis';
  elseif strcmp(lower(n),'output')
    a = [0 1];
  else
    error('Unrecognized property.')
  end
else
  if nargin==2
    [nr,nc] = size(n);
    n = n .* (b*ones(1,nc));
  end
  a = exp(-(n.*n));
end
