function [r]=corre(x,y)
% ���ϵ�������������ϵ��
% ���룺x������ף�y���αȹ��ף�
% �����r,���ϵ��
	r=(sum(x.*y))^2/(sum(x.*x)*sum(y.*y));
	
end
