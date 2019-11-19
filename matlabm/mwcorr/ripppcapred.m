function [tun] = ripppcapred(a, p , w )
%RIPPpcapred   Predicts pricipal concentrations of unknown spectra.

% Where:
%
%  x    is the residual after decomposition of the unknown spectrum or spectra
%  a    is the unknown spectrum (or spectra)
%  p    is the matrix of spectral factors
%  b    is the vector containing the inner relationships
% 
[i,j]=size(a);

[maxrank, h] = size(p);
n = maxrank;

for k=1:j
    x = a(:,k)';
    c=0;
    for h=1:n
	   t(:,h) = x * w(h,:)';
	   x = x - t(:,h) * p(h,:);
    end
end
tun=t;
