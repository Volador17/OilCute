%��ֵ���Ļ���������У������Ʒ���������У��������Ʒ����ʹ�ó���mcentp��
% input 
% x:����Ⱦ���
% output 
% xc������������Ⱦ���
% mx��ƽ�����ס�

function [xc,mx] = mcent(x)
	[m,n] = size(x);
	mx = mean(x,2);
	xc = x-mx*ones(1,n);
end